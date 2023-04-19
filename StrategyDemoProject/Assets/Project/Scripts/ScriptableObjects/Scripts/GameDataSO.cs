using UnityEngine;

namespace StrategyDemoProject
{
    [CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
    public class GameDataSO : ScriptableObject
    {
        public BarrackDataSO HeavyBarrackSO;
        public BarrackDataSO MediumBarrackSO;
        public BarrackDataSO LightBarrackSO;

        public PowerPlantDataSO BigPowerPlantSO;
        public PowerPlantDataSO MediumPowerPlantSO;
        public PowerPlantDataSO SmallPowerPlantSO;
    } 
}
