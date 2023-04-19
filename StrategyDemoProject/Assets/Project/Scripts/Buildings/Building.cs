using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StrategyDemoProject
{
    /// <summary>
    /// Building base class.
    /// </summary>
    public class Building : MonoBehaviour, IDamageable, IPlaceable, ISelectable
    {
        /// <summary>
        /// Unit background visual sprite renderer.
        /// </summary>
        [SerializeField] private SpriteRenderer visualSpriteRenderer;

        /// <summary>
        /// Health bar Sprite GameObject.
        /// </summary>
        [SerializeField] private GameObject healtBar;

        private int maxHealth;

        public PlaceableDataSO PlaceableDataSO { get; protected set; }
        public BuildingType BuildingType { get; protected set; }
        public int HealthPoint { get; protected set; }
        public int CellSizeX { get; protected set; }
        public int CellSizeY { get; protected set; }
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
        /// For run when building instantiated.
        /// </summary>
        /// <param name="data">Building data.</param>
        public virtual void Occur(BuildingDataSO data)
        {
            PlaceableDataSO = data;
            BuildingType = data.BuildingType;
            HealthPoint = data.Health;
            maxHealth = data.Health;
            CellSizeX = data.CellSizeX;
            CellSizeY = data.CellSizeY;
            healtBar.transform.localScale = Vector3.one;
        }

        public virtual void Produce() { }

        public virtual void TakeDamage(int damage)
        {
            HealthPoint -= damage;
            float healthBarValue = (float)HealthPoint / (float)maxHealth;
            healtBar.transform.localScale = new Vector3(healthBarValue, 1f, 1f);
        }

        /// <summary>
        /// Destroy and set available occupying grid path points.
        /// </summary>
        public virtual void Demolish()
        {
            List<PathNode> gridPoints = GetGridPointList();

            foreach (PathNode point in gridPoints)
            {
                point.PlacedObject = null;
                point.IsWalkable = true;
            }

            PoolManager.Instance.SendObjectToPool(gameObject);
        }

        public virtual void Settle(Vector3 position)
        {
            transform.position = position;
        }

        public void Destroy()
        {
            EventManager.OnPlaceableObjectDestroyed?.Invoke(this);
            EventManager.OnSelectableObjectDestroyed?.Invoke(this);

            Demolish();
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

        public void SetVisualColor(Color color)
        {
            visualSpriteRenderer.color = color;
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
    }
}
