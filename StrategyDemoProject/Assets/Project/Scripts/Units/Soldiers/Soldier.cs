using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class Soldier : Unit
    {
        public SoldierDataSO SoldierData { get; private set; }


        public override void Spawn(UnitDataSO data)
        {
            base.Spawn(data);

            SoldierData = data as SoldierDataSO;
            Damage = SoldierData.Damage;
        }
    } 
}
