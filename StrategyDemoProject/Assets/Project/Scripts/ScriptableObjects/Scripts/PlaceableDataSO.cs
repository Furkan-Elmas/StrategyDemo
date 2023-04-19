using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StrategyDemoProject
{
    public class PlaceableDataSO : ScriptableObject
    {
        public GameObject Prefab;
        public Sprite ImageSprite;
        public int Health;
        public int CellSizeX;
        public int CellSizeY;
    } 
}
