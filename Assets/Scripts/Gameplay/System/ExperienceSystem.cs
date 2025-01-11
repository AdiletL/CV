using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ExperienceSystem : IInitializable, IDisposable
{
    private GameUnits gameUnits;
    
    [Inject]
    public void Construct(GameUnits gameUnits)
    {
        this.gameUnits = gameUnits;
    }
    
    public void Initialize()
    {
        Unit.UnitExperience.OnGiveAoeExperience += OnGiveAoeExperience;
    }

    public void Dispose()
    {
        Unit.UnitExperience.OnGiveAoeExperience -= OnGiveAoeExperience;
    }

    private void OnGiveAoeExperience(Unit.AoeExperienceInfo info)
    {
        var experienceUnits = gameUnits.GetUnits<IUnitExperience>();
        var sortingUnits = new List<IUnitExperience>();
     
        var damagingExperience = info.Damaging.GetComponent<IUnitExperience>();
        
        for (int i = experienceUnits.Count - 1; i >= 0; i--)
        {
            if (experienceUnits[i] == null)
            {
                experienceUnits.RemoveAt(i);
                continue;
            }

            if (experienceUnits[i] is MonoBehaviour monoBehaviour
                && monoBehaviour.TryGetComponent(out IHealth healthComponent)
                && healthComponent.IsLive
                && experienceUnits[i].IsTakeExperience
                && experienceUnits[i].IsRangeTakeExperience(info.GameObject))
            {
                if(damagingExperience != null && experienceUnits[i] != damagingExperience)
                    sortingUnits.Add(experienceUnits[i]);
            }
        }

        if(sortingUnits.Count == 0) return;
        
        int resultExperience = info.TotalExperience / sortingUnits.Count;
        foreach (var VARIABLE in sortingUnits)
        {
            VARIABLE.AddExperience(resultExperience);
        }
    }
}
