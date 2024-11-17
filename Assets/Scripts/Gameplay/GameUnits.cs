using System;
using System.Collections.Generic;
using UnityEngine;

public class GameUnits
{
    private Dictionary<UnitType, List<IUnit>> allUnits = new();
    private List<IUnitExperience> experienceUnits = new();
    
    public void AddUnits(params IUnit[] units)
    {
        foreach (var VARIABLE in units)
        {
            if (!allUnits.ContainsKey(VARIABLE.UnitType))
            {
                allUnits[VARIABLE.UnitType] = new List<IUnit>();
            }

            allUnits[VARIABLE.UnitType].Add(VARIABLE);

            if (VARIABLE is MonoBehaviour monoBehaviour)
            {
                if (monoBehaviour.TryGetComponent(out IUnitExperience unitExperience))
                    experienceUnits.Add(unitExperience);
            }
        }
    }

    public void RemoveUnits(params IUnit[] units)
    {
        foreach (var VARIABLE in units)
        {
            allUnits[VARIABLE.UnitType].Remove(VARIABLE);
            if (VARIABLE is MonoBehaviour monoBehaviour)
            {
                if (monoBehaviour.TryGetComponent(out IUnitExperience unitExperience))
                    experienceUnits.Add(unitExperience);
            }
        }
    }

    public List<IUnitExperience> GetListUnitsExperience()
    {
        return experienceUnits;
    }
}
