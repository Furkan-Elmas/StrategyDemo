using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class PathNode
    {
        private Grid<PathNode> grid;

        public PathNode NodeCameFrom { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get; set; }
        public bool IsWalkable { get; set; }
        public IPlaceable PlacedObject { get; set; }
        public Vector3 Position { get; set; }
        public int X { get; }
        public int Y { get; }

        public PathNode(Grid<PathNode> grid, int x, int y)
        {
            this.grid = grid;
            Position = new Vector2(x, y);
            X = x;
            Y = y;
            IsWalkable = true;
        }

        public void CalculateFCost()
        {
            FCost = GCost + HCost;
        }
    }
}
