using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComponentsInGameObjects
{
    [field: SerializeField] public GameObject[] gameObjects { get; private set; }

    private readonly List<Type> componentTypes = new();
    
    public void Initialize()
    {
            
    }
    
    public T GetComponentFromArray<T>() where T : class
    {
        foreach (var type in componentTypes)
        {
            if (type == typeof(T))
                return GetComponentInGameObjects<T>();
        }
        
        var component = GetComponentInGameObjects<T>();
        if (component != null)
            componentTypes.Add(typeof(T));

        return component;
    }
    
    private T GetComponentInGameObjects<T>() where T : class
    {
        foreach (var obj in gameObjects)
        {
            if (obj.TryGetComponent(out T component))
                return component;
        }
        return null;
    }
    
    public List<T> GetComponentsInGameObjects<T>()
    {
        List<T> components = new();
        foreach (var obj in gameObjects)
        {
            components.AddRange(obj.GetComponents<T>());
        }
        return components;
    }
    
    public bool TryGetComponentFromArray<T>(out T component) where T : class
    {
        foreach (var type in componentTypes)
        {
            if (type == typeof(T))
            {
                component = GetComponentInGameObjects<T>();
                return component != null;
            }
        }

        if (TryGetComponentInGameObjects(out component))
        {
            componentTypes.Add(typeof(T));
            return true;
        }
        
        component = null;
        return false;
    }
    
    private bool TryGetComponentInGameObjects<T>(out T component) where T : class
    {
        foreach (var obj in gameObjects)
        {
            if (obj.TryGetComponent(out component))
            {
                return true;
            }
        }
        component = null;
        return false;
    }
}
