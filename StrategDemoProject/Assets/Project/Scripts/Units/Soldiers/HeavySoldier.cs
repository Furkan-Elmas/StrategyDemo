using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class HeavySoldier : Soldier
    {
        public override void Spawn(UnitDataSO data)
        {
            base.Spawn(data);

            UnitType = UnitType.HeavySoldier;
        }
    }
}
