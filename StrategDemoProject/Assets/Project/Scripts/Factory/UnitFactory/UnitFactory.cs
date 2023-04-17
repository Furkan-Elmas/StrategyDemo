using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace StrategyDemoProject
{
    /// <summary>
    /// Unit Factory Singleton class.
    /// </summary>
    public class UnitFactory : Singleton<UnitFactory>
    {
        private static Dictionary<string, Type> unitsByType;

        protected override void Awake()
        {
            base.Awake();

            InitializeUnitFactory();
        }

        /// <summary>
        /// Get instance of subclass by unit type.
        /// </summary>
        /// <param name="unitType"></param>
        /// <returns>Instance of requested subclass or null</returns>
        public Unit GetUnit(Type unitType, UnitDataSO data)
        {
            // If Dictionary contains given type then creates a new instance of subclass and returns it. If it doesn't then returns null.
            if (unitsByType.ContainsKey(unitType.Name))
            {
                GameObject buildingObj = PoolManager.Instance.GetPooledObject(unitType, data.Prefab);

                Unit unit = buildingObj.GetComponent(unitType) as Unit;

                unit.Spawn(data);

                return unit;
            }

            return null;
        }

        /// <summary>
        /// Create objects of factory.
        /// </summary>
        private static void InitializeUnitFactory()
        {
            // Gets all subclass types inherited from Unit abstract class.
            IEnumerable<Type> factoriesByType = Assembly.GetAssembly(typeof(Unit))
                .GetTypes()
                .Where(T => T.IsClass && !T.IsAbstract && T.IsSubclassOf(typeof(Unit)));

            // Create new Dictionary.
            unitsByType = new Dictionary<string, Type>();

            // Fill Dictionary with subclass types of Unit superclass.
            foreach (Type type in factoriesByType)
            {
                unitsByType.Add(type.Name, type);
            }
        }
    }
}
