using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    [CreateAssetMenu(fileName = "SoldierData", menuName = "ScriptableObjects/SoldierData")]
    public class SoldierDataSO : UnitDataSO
    {
        public int Damage;
    }
}
