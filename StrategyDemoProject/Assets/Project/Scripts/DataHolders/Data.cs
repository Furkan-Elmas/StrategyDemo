using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace StrategyDemoProject
{
    [Serializable]
    public struct AllBuildingsData
    {
        public BarrackDataSO HeavyBarrack;
        public BarrackDataSO MediumBarrack;
        public BarrackDataSO LightBarrack;

        public PowerPlantDataSO SmallPowerPlant;
        public PowerPlantDataSO MediumPowerPlant;
        public PowerPlantDataSO BigPowerPlant;
    }

    [Serializable]
    public struct UnitsData
    {
        public SoldierData HeavySoldier;
        public SoldierData MediumSoldier;
        public SoldierData LightSoldier;
    }

    [Serializable]
    public struct SoldierData
    {
        public int Health;
        public int Damage;
    }

    [Serializable]
    public struct UIObjects
    {
        public GameObject InformationPanel;
        public Image SelectedObjectImage;
        public Image SelectedObjectProductImage;
        public TMP_Text SelectedObjectNameText;
        public TMP_Text SelectedObjectProductNameText;
        public UnitButton SelectableObjectProductButton;
    }
}