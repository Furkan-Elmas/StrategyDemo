using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace StrategyDemoProject
{
    /// <summary>
    /// Building Factory Singleton class.
    /// </summary>
    public class BuildingFactory : Singleton<BuildingFactory>
    {
        private static Dictionary<string, Type> buildingsByType;

        protected override void Awake()
        {
            base.Awake();

            InitializeBuildingFactory();
        }

        /// <summary>
        /// Get instance of subclass by building type.
        /// </summary>
        /// <param name="buildingType">Type of building.</param>
        /// <param name="data">Scriptable Object that contains building data.</param>
        /// <returns>Instance of requested subclass or null</returns>
        public Building GetBuilding(Type buildingType, BuildingDataSO data)
        {
            // If Dictionary contains given type then creates a new instance of subclass and returns it. If it doesn't then returns null.
            if (buildingsByType.ContainsKey(buildingType.Name))
            {
                GameObject buildingObj = PoolManager.Instance.GetPooledObject(buildingType, data.Prefab);

                Building building = buildingObj.GetComponent(buildingType) as Building;

                building.Occur(data);

                return building;
            }

            return null;
        }

        /// <summary>
        /// Create objects of factory.
        /// </summary>
        private void InitializeBuildingFactory()
        {
            // Gets all subclass types inherited from Building class.
            IEnumerable<Type> factoriesByType = Assembly.GetAssembly(typeof(Building))
                .GetTypes()
                .Where(T => T.IsClass && !T.IsAbstract && T.IsSubclassOf(typeof(Building)));

            // Create new Dictionary.
            buildingsByType = new Dictionary<string, Type>();

            // Fill Dictionary with subclass types of Building superclass.
            foreach (Type type in factoriesByType)
            {
                buildingsByType.Add(type.Name, type);
            }
        }
    }
}
