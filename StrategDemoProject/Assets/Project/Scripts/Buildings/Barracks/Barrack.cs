using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class Barrack : Building
    {
        public BarrackDataSO BarrackData { get; private set; }
        public SoldierDataSO ProductionData { get; private set; }
        public SpawnPoint SpawnPoint { get; set; }


        protected virtual void Awake()
        {
            SpawnPoint = GetComponentInChildren<SpawnPoint>();
        }

        public override void Occur(BuildingDataSO data)
        {
            base.Occur(data);

            BarrackData = data as BarrackDataSO;
            ProductionData = BarrackData.ProductionData;
        }

        public override void Settle(Vector3 position)
        {
            base.Settle(position);

            SetSpawnPoint();
        }

        /// <summary>
        /// Changes spawn point position to specified position.
        /// </summary>
        /// <param name="position"></param>
        public void ChangeSpawnPoint(Vector3 position)
        {
            Grid<PathNode> grid = GridController.Instance.Grid;

            grid.GetXY(position, out int x, out int y);

            PathNode point = grid.GetGridObject(x, y);

            if (!point.IsWalkable)
                return;

            SpawnPoint.GridObject = point;
            SpawnPoint.transform.position = grid.GetWorldPosition(point.X, point.Y);
        }

        /// <summary>
        /// Gets nearest grid path point to spawn point.
        /// </summary>
        /// <param name="position">Spawn point position.</param>
        /// <returns>Nearest path point.</returns>
        protected PathNode GetNearestSpawnPoint(Vector3 position)
        {
            List<PathNode> points = GetAvailablePointsAround();
            PathNode nearestPoint = null;
            float shortestDistance = Mathf.Infinity;

            foreach (PathNode point in points)
            {
                if (!point.IsWalkable)
                    continue;

                Vector3 worldPoint = GridController.Instance.Grid.GetWorldPosition(point.X, point.Y);
                float distance = Vector3.Distance(position, worldPoint);

                if (shortestDistance >= distance)
                {
                    shortestDistance = distance;
                    nearestPoint = point;
                }
            }

            return nearestPoint;
        }

        /// <summary>
        /// Sets spawn point as any available point around building.
        /// </summary>
        protected void SetSpawnPoint()
        {
            SpawnPoint.GridObject = null;

            Grid<PathNode> grid = GridController.Instance.Grid;

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
                        SpawnPoint.GridObject = point;
                        SpawnPoint.transform.position = grid.GetWorldPosition(point.X, point.Y);

                        break;
                    }
                }
            }
        }
    }
}
