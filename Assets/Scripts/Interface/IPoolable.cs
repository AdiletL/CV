using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    Transform poolParent { get; }
    List<GameObject> PoolObjects { get; }
    
    public GameObject GetObject<T>();
    
    public void ReturnToPool(GameObject obj);
}
