using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class HeavyBarrack : Barrack
    {
        /// <summary>
        /// Creates heavy soldier unit settle nearest position to spawn point and moves it to there.
        /// </summary>
        public override void Produce()
        {
            if (SpawnPoint.GridObject == null || !SpawnPoint.GridObject.IsWalkable)
                return;

            Grid<PathNode> grid = GridController.Instance.Grid;
            Vector3 movePosition = grid.GetWorldPosition(SpawnPoint.GridObject.X, SpawnPoint.GridObject.Y);
            PathNode firstSpawnPoint = GetNearestSpawnPoint(movePosition);

            if (firstSpawnPoint == null)
                return;

            Vector3 firstSpawnPosition = grid.GetWorldPosition(firstSpawnPoint.X, firstSpawnPoint.Y);

            HeavySoldier heavySoldier = UnitFactory.Instance.GetUnit(typeof(HeavySoldier), ProductionData) as HeavySoldier;

            firstSpawnPoint.PlacedObject = heavySoldier;
            firstSpawnPoint.IsWalkable = false;

            heavySoldier.Settle(firstSpawnPosition);
            heavySoldier.Move(movePosition);
        }
    }
}
