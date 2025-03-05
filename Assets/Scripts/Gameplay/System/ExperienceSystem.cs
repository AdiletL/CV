using System;
using Unit;
using Unit.Character.Player;
using UnityEngine;
using Zenject;

public class ExperienceSystem
{
    [Inject] private GameUnits gameUnits;

    public ExperienceSystem()
    {
        Unit.UnitExperience.OnGiveAoeExperience += OnGiveAoeExperience;
    }

    ~ExperienceSystem()
    {
        Unit.UnitExperience.OnGiveAoeExperience -= OnGiveAoeExperience;
    }

    private void OnGiveAoeExperience(Unit.AoeExperienceInfo info)
    {
        var experienceUnits = gameUnits.GetUnits<Unit.UnitExperience>();
        float distanceSqr = 0;
        
        for (int i = experienceUnits.Count - 1; i >= 0; i--)
        {
            if (!experienceUnits[i] 
                || !experienceUnits[i].gameObject.activeSelf
                || !experienceUnits[i].IsTakeExperience
                || !experienceUnits[i].TryGetComponent(out IHealth health)
                || !health.IsLive)
            {
                experienceUnits.RemoveAt(i);
                continue;
            }
            
            distanceSqr = experienceUnits[i].RangeTakeExperience * experienceUnits[i].RangeTakeExperience;
            if (!Calculate.Distance.IsNearUsingSqr(info.Owner.transform.position,
                    experienceUnits[i].transform.position, distanceSqr))
            {
                experienceUnits.RemoveAt(i);
            }
        }

        if(experienceUnits.Count == 0) return;
        
        int resultExperience = info.TotalExperience / experienceUnits.Count;
        foreach (var VARIABLE in experienceUnits)
        {
            VARIABLE.AddExperience(resultExperience);
        }
    }
}
