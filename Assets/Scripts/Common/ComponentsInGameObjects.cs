using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComponentsInGameObjects
{
    [field: SerializeField] public GameObject[] gameObjects { get; private set; }

    public void Initialize()
    {
        
    }

    public T GetComponentInGameObjects<T>()
    {
        foreach (var item in gameObjects)
        {
            var component = item.GetComponent<T>();
            if (component == null) continue;
            else return component;
        }
        return default;
    }
    public List<T> GetComponentsInGameObjects<T>()
    {
        List<T> list = new();
        foreach (var item in gameObjects)
        {
            var components = item.GetComponents<T>();
            list.AddRange(components);
        }

        return list;
    }
    
    public bool TryGetComponentInGameObjects<T>(out T component) where T : Component
    {
        foreach (var item in gameObjects)
        {
            component = item.GetComponent<T>();
            if (component != null)
            {
                return true;
            }
        }
        component = null;
        return false;
    }
}
