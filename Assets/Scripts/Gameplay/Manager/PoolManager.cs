using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Gameplay.Manager
{
    public class PoolManager : MonoBehaviour, IManager, IPoolable
    {
        [Inject] private DiContainer diContainer;
        
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
                    PoolObjects.RemoveAt(i);
                    return poolObject;
                }
            }

            foreach (var VARIABLE in poolObjects)
            {
                var poolObject = VARIABLE;
                var component = poolObject.GetComponent<T>();
                if (component != null)
                {
                    var newGameObject = diContainer.InstantiatePrefab(poolObject);
                    return newGameObject;
                }
            }

            throw new NullReferenceException();
        }

        public void ReturnToPool(GameObject obj)
        {
            obj.transform.SetParent(poolParent);
            obj.SetActive(false);
            PoolObjects.Add(obj);
        }
    }
}