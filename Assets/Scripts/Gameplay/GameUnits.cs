using System.Collections.Generic;
using System.Linq;


public class GameUnits
{
    private List<IUnit> units = new ();
    
    public void AddUnits(params IUnit[] newUnits)
    {
        foreach (var unit in newUnits)
        {
            if (!units.Contains(unit))
            {
                units.Add(unit);
            }
        }
    }
    
    public void RemoveUnits(params IUnit[] removeUnits)
    {
        foreach (var unit in removeUnits)
        {
            units.Remove(unit);
        }
    }
    
    public List<T> GetUnits<T>() where T : class
    {
        return units.OfType<T>().ToList();
    }
}