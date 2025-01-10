using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IPoolable
{
    Transform poolParent { get; }
    List<GameObject> PoolObjects { get; }
    
    public Task<GameObject> GetObjectAsync<T>();
    
    public void ReturnToPool(GameObject obj);
}
