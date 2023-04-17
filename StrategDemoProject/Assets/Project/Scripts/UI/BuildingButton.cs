using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class BuildingButton : MonoBehaviour
    {
        public void GetBuilding(BuildingDataSO buildingDataSO)
        {
            BuildingType buildingType = buildingDataSO.BuildingType;

            IPlaceable building = buildingType switch
            {
                BuildingType.HeavyBarrack => BuildingFactory.Instance.GetBuilding(typeof(HeavyBarrack), buildingDataSO),
                BuildingType.MediumBarrack => BuildingFactory.Instance.GetBuilding(typeof(MediumBarrack), buildingDataSO),
                BuildingType.LightBarrack => BuildingFactory.Instance.GetBuilding(typeof(LightBarrack), buildingDataSO),
                BuildingType.SmallPowerPlant => BuildingFactory.Instance.GetBuilding(typeof(SmallPowerPlant), buildingDataSO),
                BuildingType.MediumPowerPlant => BuildingFactory.Instance.GetBuilding(typeof(MediumPowerPlant), buildingDataSO),
                BuildingType.BigPowerPlant => BuildingFactory.Instance.GetBuilding(typeof(BigPowerPlant), buildingDataSO),
                _ => null
            };

            EventManager.OnPlaceableObjectPicked?.Invoke(building);
        }
    }
}