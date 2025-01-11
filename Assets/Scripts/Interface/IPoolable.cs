using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IPoolable
{
    Transform poolParent { get; }
    List<GameObject> PoolObjects { get; }
    
    public UniTask<GameObject> GetObjectAsync<T>();
    
    public void ReturnToPool(GameObject obj);
}
