using System;
using UnityEngine;

namespace StrategyDemoProject
{
    [CreateAssetMenu(fileName = "BarrackData", menuName = "ScriptableObjects/BarrackData")]
    public class BarrackDataSO : BuildingDataSO
    {
        public SoldierDataSO ProductionData;
    }
}
