using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Manager
{
    public class PoolManager : MonoBehaviour, IManager, IPoolable
    {
        [SerializeField] private List<GameObject> poolObjects;
        
        public Transform poolParent { get; private set; }
        public List<GameObject> PoolObjects { get; private set; } = new();
        
        
        public void Initialize()
        {
            poolParent = new GameObject("PoolParent").transform;
        }

        public GameObject GetObject<T>()
        {
            for(int i = PoolObjects.Count - 1; i >= 0; i--)
            {
                var poolObject = PoolObjects[i];
                var component = poolObject.GetComponent<T>();
                if (component != null && !poolObject.activeInHierarchy)
                {
                    poolObject.transform.SetParent(null);
                    poolObject.SetActive(true);
                    return poolObject;
                }
            }

            foreach (var VARIABLE in poolObjects)
            {
                var poolObject = VARIABLE;
                var component = poolObject.GetComponent<T>();
                if (component != null)
                {
                    var newGameObject = Instantiate(poolObject);
                    return newGameObject;
                }
            }

            throw new NullReferenceException();
        }

        public void ReturnToPool(GameObject obj)
        {
            obj.transform.SetParent(poolParent);
            obj.SetActive(false);
        }
    }
}