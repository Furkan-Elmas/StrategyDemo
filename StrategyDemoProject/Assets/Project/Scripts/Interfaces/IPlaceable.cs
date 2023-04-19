using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public interface IPlaceable
    {
        public Transform Transform { get; }
        public PlaceableDataSO PlaceableDataSO { get; }

        public void Destroy();
        public void Settle(Vector3 position);
        public void SetVisualColor(Color color);
        public List<Vector2Int> GetGridPositionList(Vector2Int offset);
        public List<PathNode> GetGridPointList();
    }
}