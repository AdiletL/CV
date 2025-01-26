using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameUnits
{
    private List<GameObject> units = new ();
    
    public void AddUnits(params GameObject[] newUnits)
    {
        foreach (var unit in newUnits)
        {
            if (!units.Contains(unit) && 
                unit.TryGetComponent(out IUnit unitComponent))
            {
                units.Add(unit);
            }
        }
    }
    
    public void RemoveUnits(params GameObject[] removeUnits)
    {
        foreach (var unit in removeUnits)
        {
            units.Remove(unit);
        }
    }
    
    public List<T> GetUnits<T>()
    {
        List<T> result = new ();

        for (int i = units.Count - 1; i >= 0; i--)
        {
            if (units[i] == null)
            {
                units.RemoveAt(i);
                continue;
            }
            
            if(units[i].TryGetComponent(out T unitComponent)) 
                result.Add(unitComponent);
        }

        return result;
    }
}