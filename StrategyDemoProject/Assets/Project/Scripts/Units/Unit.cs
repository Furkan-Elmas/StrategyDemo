using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace StrategyDemoProject
{
    /// <summary>
    /// Unit base class.
    /// </summary>
    public class Unit : MonoBehaviour, IDamageable, IPlaceable, ISelectable
    {
        /// <summary>
        /// Unit background visual sprite renderer.
        /// </summary>
        [SerializeField] private SpriteRenderer visualSpriteRenderer;

        /// <summary>
        /// Health bar Sprite GameObject.
        /// </summary>
        [SerializeField] private GameObject healtBar;

        [SerializeField] private Animator animator;

        /// <summary>
        /// Unit front visual GameObject.
        /// </summary>
        [SerializeField] private GameObject visual;

        /// <summary>
        /// Coroutine for handle movement.
        /// </summary>
        private Coroutine movementCoroutine;

        /// <summary>
        /// Coroutine for handle attack.
        /// </summary>
        private Coroutine attackCoroutine;

        /// <summary>
        /// Unit's current grid path point.
        /// </summary>
        private PathNode currentPathPoint;

        private int maxHealth;

        public PlaceableDataSO PlaceableDataSO { get; protected set; }
        public UnitType UnitType { get; protected set; }
        public int HealthPoint { get; protected set; }
        public int CellSizeX { get; protected set; }
        public int CellSizeY { get; protected set; }
        public int Damage { get; protected set; }
        public int Speed { get; protected set; }
        public float AttackFrequency { get; protected set; }
        public Transform Transform { get => transform; }


        private void OnEnable()
        {
            // Subscribe to selectable dictionary pool for getting ISelectable interface.
            SelectablePool.Subscribe(transform, this);

            // Subscribe to damageable dictionary pool for getting IDamageable interface.
            DamageablePool.Subscribe(transform, this);
        }

        private void OnDisable()
        {
            SelectablePool.Unsubscribe(transform);
            DamageablePool.Unsubscribe(transform);
        }

        /// <summary>
        /// For run when unit instantiated.
        /// </summary>
        /// <param name="data">Unit data.</param>
        public virtual void Spawn(UnitDataSO data)
        {
            PlaceableDataSO = data;
            UnitType = data.UnitType;
            HealthPoint = data.Health;
            maxHealth = data.Health;
            CellSizeX = data.CellSizeX;
            CellSizeY = data.CellSizeY;
            Speed = data.Speed;
            AttackFrequency = data.AttackFrequency;
            healtBar.transform.localScale = Vector3.one;
        }

        public virtual void Attack(IDamageable target)
        {
            List<PathNode> availablePointAroundTarget = target.GetAvailablePointsAround();
            Vector3 shortestPoint = Vector3.zero;
            float shortestDistance = Mathf.Infinity;

            foreach (PathNode point in availablePointAroundTarget)
            {
                Vector3 worldPoint = GridController.Instance.Grid.GetWorldPosition(point.X, point.Y);
                float distance = Vector3.Distance(transform.position, worldPoint);

                if (shortestDistance >= distance)
                {
                    shortestDistance = distance;
                    shortestPoint = worldPoint;
                }
            }

            Move(shortestPoint, () =>
                {
                    attackCoroutine = StartCoroutine(StartAttack(target));
                });
        }

        public virtual void TakeDamage(int damage)
        {
            HealthPoint -= damage;
            float healthBarValue = (float)HealthPoint / (float)maxHealth;
            healtBar.transform.localScale = new Vector3(healthBarValue, 1f, 1f);
        }

        public virtual void Die()
        {
            List<PathNode> gridPoints = GetGridPointList();

            foreach (PathNode point in gridPoints)
            {
                point.PlacedObject = null;
                point.IsWalkable = true;
            }

            PoolManager.Instance.SendObjectToPool(gameObject);
        }

        public virtual void Move(Vector3 position, Action onPathOver = null)
        {
            Grid<PathNode> grid = GridController.Instance.Grid;

            grid.GetXY(position, out int endX, out int endY);
            grid.GetXY(transform.position, out int startX, out int startY);

            List<PathNode> path = GridController.Instance.PathFinding.FindPath(startX, startY, endX, endY);

            if (path == null)
                return;

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);

            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
                currentPathPoint.IsWalkable = true;
            }

            movementCoroutine = StartCoroutine(MovementCoroutine(path, onPathOver));
        }

        public virtual void Settle(Vector3 position)
        {
            transform.position = position;
        }

        public void Destroy()
        {
            EventManager.OnPlaceableObjectDestroyed?.Invoke(this);
            EventManager.OnSelectableObjectDestroyed?.Invoke(this);

            Die();
        }

        public void OnSelect()
        {

        }

        public void OnDeselect()
        {

        }

        /// <summary>
        /// Gets current occupying grid positions.
        /// </summary>
        /// <param name="offset">World position offset</param>
        /// <returns>Position list.</returns>
        public List<Vector2Int> GetGridPositionList(Vector2Int offset)
        {
            List<Vector2Int> gridPositionList = new List<Vector2Int>();

            for (int x = 0; x < CellSizeX; x++)
            {
                for (int y = 0; y < CellSizeY; y++)
                {
                    gridPositionList.Add(offset + new Vector2Int(x, y));
                }
            }

            return gridPositionList;
        }

        /// <summary>
        /// Gets current occupying grid path points.
        /// </summary>
        /// <returns>Grid path point list.</returns>
        public List<PathNode> GetGridPointList()
        {
            Grid<PathNode> grid = GridController.Instance.Grid;
            List<PathNode> points = new List<PathNode>();

            for (int i = 0; i < CellSizeX; i++)
            {
                for (int j = 0; j < CellSizeY; j++)
                {
                    grid.GetXY(transform.position, out int x, out int y);

                    PathNode point = grid.GetGridObject(i + x, j + y);

                    points.Add(point);
                }
            }

            return points;
        }

        public void SetVisualColor(Color color)
        {
            visualSpriteRenderer.color = color;
        }

        /// <summary>
        /// Gets available grid path point around unit.
        /// </summary>
        /// <returns>Grid path point list.</returns>
        public List<PathNode> GetAvailablePointsAround()
        {
            Grid<PathNode> grid = GridController.Instance.Grid;
            List<PathNode> points = new List<PathNode>();

            for (int i = -1; i < CellSizeX + 1; i++)
            {
                for (int j = -1; j < CellSizeY + 1; j++)
                {
                    if (j != -1 && j != CellSizeY && i != -1 && i != CellSizeX)
                        continue;

                    grid.GetXY(transform.position, out int x, out int y);

                    PathNode point = grid.GetGridObject(i + x, j + y);

                    if (point != default(PathNode) && point.IsWalkable)
                    {
                        points.Add(point);
                    }
                }
            }

            return points;
        }

        private IEnumerator MovementCoroutine(List<PathNode> path, Action onPathOver = null)
        {
            Vector3 positionGoTo;
            PathNode pathCameFrom;
            PathNode pathGoTo;

            animator.SetBool("Moving", true);

            int i = 0;
            foreach (PathNode node in path)
            {
                // End coroutine if current path point is end of road.
                if (i == path.Count - 1)
                {
                    OnPathOver(onPathOver);
                    yield break;
                }

                pathCameFrom = path[i];
                pathGoTo = path[i + 1];

                // End coroutine if next path point is not walkable.
                if (!pathGoTo.IsWalkable)
                {
                    OnPathOver();
                    yield break;
                }

                currentPathPoint = pathGoTo;
                positionGoTo = GridController.Instance.Grid.GetWorldPosition(pathGoTo.X, pathGoTo.Y);

                pathCameFrom.IsWalkable = true;
                pathGoTo.IsWalkable = false;
                SetVisualRotation(pathCameFrom, pathGoTo);

                // While loop for interpolate position of unit to position to go.
                while (!Equals(transform.position, positionGoTo))
                {
                    transform.position = Vector3.MoveTowards(transform.position, positionGoTo, Time.deltaTime * Speed);

                    yield return null;
                }
                i++;
            }
        }

        private IEnumerator StartAttack(IDamageable target)
        {
            // While loop for reducing health point of target below zero.
            while (target.HealthPoint > 0)
            {
                animator.SetTrigger("Attack");

                target.TakeDamage(Damage);

                if (target.HealthPoint <= 0)
                {
                    target.Destroy();
                    yield break;
                }

                yield return new WaitForSeconds(AttackFrequency);
            }
        }

        /// <summary>
        /// Sets unit direction of view.
        /// </summary>
        /// <param name="pathCameFrom">Grid path point of coming from.</param>
        /// <param name="pathGoTo">Grid path point of going to.</param>
        private void SetVisualRotation(PathNode pathCameFrom, PathNode pathGoTo)
        {
            if (pathCameFrom.X > pathGoTo.X)
                visual.transform.eulerAngles = Vector3.zero;
            else
                visual.transform.eulerAngles = Vector3.up * 180;
        }

        private void OnPathOver(Action onPathOver = null)
        {
            animator.SetBool("Moving", false);

            if (onPathOver != null)
                onPathOver.Invoke();
        }
    }
}
