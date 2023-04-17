using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private GameDataSO data;

        public GameDataSO Data { get => data; }
    }
}
