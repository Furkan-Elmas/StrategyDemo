using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class MediumSoldier : Soldier
    {
        public override void Spawn(UnitDataSO data)
        {
            base.Spawn(data);

            UnitType = UnitType.MediumSoldier;
        }
    }
}
