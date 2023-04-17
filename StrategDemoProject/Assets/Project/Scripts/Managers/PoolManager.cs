using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class PoolManager : Singleton<PoolManager>
    {
        private Dictionary<System.Type, List<GameObject>> poolDictionary = new Dictionary<System.Type, List<GameObject>>();
        private GameObject objectPool;


        protected override void Awake()
        {
            base.Awake();

            objectPool = new GameObject("Object Pool");
        }

        public GameObject GetPooledObject(System.Type objectType, GameObject prefab)
        {
            if (poolDictionary.ContainsKey(objectType))
            {
                List<GameObject> pool = poolDictionary[objectType];

                for (int i = 0; i < pool.Count; i++)
                {
                    if (!pool[i].activeInHierarchy)
                    {
                        pool[i].SetActive(true);
                        return pool[i];
                    }
                }
                GameObject newObject = Instantiate(prefab);
                poolDictionary[objectType].Add(newObject);
                newObject.transform.SetParent(objectPool.transform);

                return newObject;
            }
            else
            {
                GameObject newObject = Instantiate(prefab);
                poolDictionary.Add(objectType, new List<GameObject>() { newObject });
                newObject.transform.SetParent(objectPool.transform);

                return newObject;
            }
        }

        public void SendObjectToPool(GameObject go)
        { 
            go.SetActive(false); 
        }
    }
}
