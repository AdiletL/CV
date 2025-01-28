using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IPoolableObject
{
    public Transform poolParent { get; }
    public List<GameObject> PoolObjects { get; }

    public void Initialize();
    
    public UniTask<GameObject> GetObjectAsync<T>();
    
    public void ReturnToPool(GameObject obj);
}
