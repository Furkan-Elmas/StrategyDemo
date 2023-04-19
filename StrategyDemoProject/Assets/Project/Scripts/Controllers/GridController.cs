using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class GridController : Singleton<GridController>
    {
        private const int GRID_WIDTH = 32;
        private const int GRID_HEIGHT = 32;
        private const int CELL_SIZE = 10;

        [SerializeField] private LayerMask selectableLayerMask;

        private PathFinding pathFinding;
        private IPlaceable pickedPlaceableObject;
        private ISelectable pickedSelectableObject;
        private Coroutine placementCoroutine;

        public Grid<PathNode> Grid { get => pathFinding.Grid; }
        public PathFinding PathFinding { get => pathFinding; }


        private void OnEnable()
        {
            EventManager.OnPlaceableObjectPicked += PlaceableObjectPick;
            EventManager.OnPlaceableObjectDestroyed += OnPlaceableDestroyed;
            EventManager.OnSelectableObjectDestroyed += OnSelectableDestroyed;
        }

        private void OnDisable()
        {
            EventManager.OnPlaceableObjectPicked -= PlaceableObjectPick;
            EventManager.OnPlaceableObjectDestroyed -= OnPlaceableDestroyed;
            EventManager.OnSelectableObjectDestroyed -= OnSelectableDestroyed;
        }

        private void Start()
        {
            pathFinding = new PathFinding(GRID_WIDTH, GRID_HEIGHT, CELL_SIZE, Vector2.zero);
        }

        private void Update()
        {
            // Controlling deselection of picked placeable object.
            HandlePlaceableObjectUnpick();

            // Controlling selection.
            HandleSelectableObjectPick();

            // Controlling selected object.
            HandleSelectableObjectOnPicked();
        }

        /// <summary>
        /// Maintaining coroutine while selected object placement.
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlacementCoroutine()
        {
            while (pickedPlaceableObject != null)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                pickedPlaceableObject.Transform.position = mousePosition + Vector3.forward;

                Grid.GetXY(mousePosition, out int x, out int y);

                Vector2Int placedObjectOrigin = new Vector2Int(x, y);

                List<Vector2Int> gridPositionList = pickedPlaceableObject.GetGridPositionList(placedObjectOrigin);

                if (CanPlace(gridPositionList))
                {
                    pickedPlaceableObject.SetVisualColor(Color.white);

                    if (Input.GetMouseButtonDown(0))
                    {
                        PlaceObject(x, y, gridPositionList);
                    }
                }
                else
                {
                    pickedPlaceableObject.SetVisualColor(Color.red);
                }

                yield return null;
            }
        }

        /// <summary>
        /// Checks if picked object can be placed in specified positions.
        /// </summary>
        /// <param name="gridPositionList">Specified position list.</param>
        /// <returns>true if placement suitable.</returns>
        private bool CanPlace(List<Vector2Int> gridPositionList)
        {
            bool canPlace = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                PathNode gridObject = Grid.GetGridObject(gridPosition.x, gridPosition.y);

                if (gridObject == null || !gridObject.IsWalkable)
                {
                    canPlace = false;
                    break;
                }
            }

            return canPlace;
        }

        /// <summary>
        /// Place picked object to specified position.
        /// </summary>
        /// <param name="x">X position of object</param>
        /// <param name="z">Y position of object</param>
        /// <param name="gridPositionList">Occupying positions of object.</param>
        private void PlaceObject(int x, int z, List<Vector2Int> gridPositionList)
        {
            Vector3 placedObjectWorldPosition = Grid.GetWorldPosition(x, z);

            pickedPlaceableObject.Settle(placedObjectWorldPosition);

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                PathNode gridObject = Grid.GetGridObject(gridPosition.x, gridPosition.y);
                gridObject.PlacedObject = pickedPlaceableObject;
                gridObject.IsWalkable = false;
            }

            pickedPlaceableObject = null;
        }

        // Set null last picked placeable object when it has been destroyed.
        private void OnPlaceableDestroyed(IPlaceable placeable)
        {
            if (placeable == pickedPlaceableObject)
            {
                pickedPlaceableObject = null;
            }
        }

        // Set null last picked selectable object when it has been destroyed.
        private void OnSelectableDestroyed(ISelectable selectable)
        {
            if (selectable == pickedSelectableObject)
            {
                pickedSelectableObject = null;
            }
        }

        private void PlaceableObjectPick(IPlaceable placeableObjbect)
        {
            if (pickedPlaceableObject != null)
                return;

            pickedPlaceableObject = placeableObjbect;

            placementCoroutine = StartCoroutine(PlacementCoroutine());
        }

        private void HandlePlaceableObjectUnpick()
        {
            if (pickedPlaceableObject == null)
                return;

            if (!Input.GetMouseButtonDown(1))
                return;

            if (pickedSelectableObject != null)
                pickedSelectableObject.SetVisualColor(Color.white);

            if (placementCoroutine != null)
                StopCoroutine(placementCoroutine);

            pickedSelectableObject = null;

            pickedPlaceableObject.Destroy();

            EventManager.OnPlaceableObjectUnpicked?.Invoke();

            pickedPlaceableObject = null;
        }

        private void HandleSelectableObjectPick()
        {
            if (pickedPlaceableObject != null)
                return;

            if (!Input.GetMouseButtonDown(0))
                return;

            RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

            if (!rayHit)
                return;

            ISelectable selectable = SelectablePool.GetSelectable(rayHit.transform);

            if (pickedSelectableObject != null)
                pickedSelectableObject.SetVisualColor(Color.white);

            selectable.SetVisualColor(Color.blue);

            pickedSelectableObject = selectable;

            if (pickedSelectableObject is Barrack)
                EventManager.OnSelectableObjectPicked?.Invoke(selectable, true);
            else if (pickedSelectableObject is PowerPlant)
                EventManager.OnSelectableObjectPicked?.Invoke(selectable, false);
        }

        private void HandleSelectableObjectOnPicked()
        {
            if (pickedSelectableObject == null)
                return;

            if (!Input.GetMouseButtonDown(1))
                return;

            if (pickedSelectableObject is Barrack)
            {
                HandleBarrackSpawnPoint();
            }
            else if (pickedSelectableObject is Unit)
            {
                if (pickedPlaceableObject != null)
                    return;

                RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                if (rayHit)
                    HandleUnitAttack(rayHit.transform);
                else
                    HandleUnitMovement();
            }
        }

        private void HandleBarrackSpawnPoint()
        {
            if (pickedSelectableObject == null)
                return;

            Barrack selectedBarrack = pickedSelectableObject as Barrack;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            selectedBarrack.ChangeSpawnPoint(mousePosition);
        }

        private void HandleUnitMovement()
        {
            if (pickedSelectableObject == null)
                return;

            Unit selectedUnit = pickedSelectableObject as Unit;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            selectedUnit.Move(mousePosition);
        }

        private void HandleUnitAttack(Transform target)
        {
            if (pickedSelectableObject == null)
                return;

            if (target == pickedSelectableObject.Transform)
                return;

            IDamageable damageableTarget = DamageablePool.GetSelectable(target);

            Unit selectedUnit = pickedSelectableObject as Unit;

            selectedUnit.Attack(damageableTarget);
        }
    }
}
