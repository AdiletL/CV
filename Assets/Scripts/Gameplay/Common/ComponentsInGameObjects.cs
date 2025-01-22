using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComponentsInGameObjects
{
    [field: SerializeField] public GameObject[] gameObjects { get; private set; }

    private List<Type> componentTypes = new List<Type>(); 
    
    public void Initialize()
    {
        
    }

    public T GetComponentFromArray<T>()
    {
        foreach (var VARIABLE in componentTypes)
        {
            if (VARIABLE is T component)
            {
                if(!componentTypes.Contains(VARIABLE))
                    componentTypes.Add(VARIABLE);
                return component;
            }
        }

        return GetComponentInGameObjects<T>();
    }

    private T GetComponentInGameObjects<T>()
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

    public bool TryGetComponentFromArray<T>(out T component) where T : class
    {
        foreach (var item in componentTypes)
        {
            if (item == typeof(T))
            {
                component = null;
                return true;
            }
        }

        if (TryGetComponentInGameObjects(out component))
        {
            component = null;
            return true;
        }
        
        return false;
    }
    
    private bool TryGetComponentInGameObjects<T>(out T component) where T : class
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
