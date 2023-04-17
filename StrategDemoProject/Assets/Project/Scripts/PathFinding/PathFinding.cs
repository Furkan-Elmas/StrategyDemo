using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class PathFinding
    {
        private const int STRAIGHT_MOVEMENT_COST = 10;
        private const int DIAGONAL_MOVEMENT_COST = 14;

        // Open list for calculated potential grid path points.
        private List<PathNode> openList;

        // Closed list for calculated non-potential grid path points.
        private List<PathNode> closedList;

        public Grid<PathNode> Grid { get; }

        // Creates grid.
        public PathFinding(int width, int height, int cellSize, Vector2 originPoint)
        {
            Grid = new Grid<PathNode>(width, height, cellSize, originPoint, (Grid<PathNode> grid, int x, int y) => new PathNode(grid, x, y));
        }

        /// <summary>
        /// Calculates shortest path points.
        /// </summary>
        /// <param name="startX">Starting position X.</param>
        /// <param name="startY">Starting position Y.</param>
        /// <param name="endX">End position X.</param>
        /// <param name="endY">End position Y.</param>
        /// <returns>Available path point list.</returns>
        public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
        {
            if (endX < 0 || endY < 0 || endX > Grid.Width - 1 || endY > Grid.Height - 1)
            {
#if UNITY_EDITOR
                Debug.Log("Invalid position.");
#endif
                return null;
            }

            PathNode startNode = Grid.GetGridObject(startX, startY);
            PathNode endNode = Grid.GetGridObject(endX, endY);

            openList = new List<PathNode>() { startNode };
            closedList = new List<PathNode>();

            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    PathNode pathNode = Grid.GetGridObject(x, y);
                    pathNode.GCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.NodeCameFrom = null;
                }
            }

            startNode.GCost = 0;
            startNode.HCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode)
                {
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                {
                    if (closedList.Contains(neighbourNode))
                        continue;

                    if (!neighbourNode.IsWalkable)
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.GCost)
                    {
                        neighbourNode.NodeCameFrom = currentNode;
                        neighbourNode.GCost = tentativeGCost;
                        neighbourNode.HCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }

            // Out of nodes on the openList
            return null;
        }

        /// <summary>
        /// Gets available path points around current path point.
        /// </summary>
        /// <param name="currentNode">Current path point.</param>
        /// <returns>All points around current point.</returns>
        private List<PathNode> GetNeighbourList(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();

            if (currentNode.X - 1 >= 0)
            {
                // Left
                neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y));
                // Left Down
                if (currentNode.Y - 1 >= 0) neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y - 1));
                // Left Up
                if (currentNode.Y + 1 < Grid.Height) neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y + 1));
            }
            if (currentNode.X + 1 < Grid.Width)
            {
                // Right
                neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y));
                // Right Down
                if (currentNode.Y - 1 >= 0) neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y - 1));
                // Right Up
                if (currentNode.Y + 1 < Grid.Height) neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y + 1));
            }
            // Down
            if (currentNode.Y - 1 >= 0) neighbourList.Add(GetNode(currentNode.X, currentNode.Y - 1));
            // Up
            if (currentNode.Y + 1 < Grid.Height) neighbourList.Add(GetNode(currentNode.X, currentNode.Y + 1));

            return neighbourList;
        }

        private PathNode GetNode(int x, int y)
        {
            return Grid.GetGridObject(x, y);
        }

        private List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);
            PathNode currentNode = endNode;
            while (currentNode.NodeCameFrom != null)
            {
                path.Add(currentNode.NodeCameFrom);
                currentNode = currentNode.NodeCameFrom;
            }
            path.Reverse();
            return path;
        }

        private int CalculateDistanceCost(PathNode a, PathNode b)
        {
            int xDistance = Mathf.Abs(a.X - b.X);
            int yDistance = Mathf.Abs(a.Y - b.Y);
            int remainingDistance = Mathf.Abs(xDistance - yDistance);

            return DIAGONAL_MOVEMENT_COST * Mathf.Min(xDistance, yDistance) + STRAIGHT_MOVEMENT_COST * remainingDistance;
        }

        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostNode = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].FCost < lowestFCostNode.FCost)
                {
                    lowestFCostNode = pathNodeList[i];
                }
            }

            return lowestFCostNode;
        }
    }
}