using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace StrategyDemoProject
{
    public class Grid<T>
    {
        private T[,] gridArray;

        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }
        public Vector2 OriginPosition { get; }

        public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<T>, int, int, T> createGridObject)
        {
            this.Width = width;
            this.Height = height;
            this.CellSize = cellSize;
            this.OriginPosition = originPosition;

            gridArray = new T[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    gridArray[x, y] = createGridObject(this, x, y);
                }
            }

            bool showDebug = true;
            if (showDebug)
            {
                for (int x = 0; x < gridArray.GetLength(0); x++)
                {
                    for (int y = 0; y < gridArray.GetLength(1); y++)
                    {
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                    }
                }
                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
            }
        }

        public Vector2 GetWorldPosition(int x, int y)
        {
            return new Vector2(x, y) * CellSize + OriginPosition;
        }

        public void GetXY(Vector2 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - OriginPosition).x / CellSize);
            y = Mathf.FloorToInt((worldPosition - OriginPosition).y / CellSize);
        }

        public void SetGridObject(int x, int y, T value)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                gridArray[x, y] = value;
            }
        }

        public void SetGridObject(Vector2 worldPosition, T value)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            SetGridObject(x, y, value);
        }

        public T GetGridObject(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                return gridArray[x, y];
            }
            else
            {
                return default(T);
            }
        }

        public T GetGridObject(Vector2 worldPosition)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return GetGridObject(x, y);
        }
    }
}
