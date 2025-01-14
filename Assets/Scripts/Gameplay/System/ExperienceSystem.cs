using System;
using Unit;
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
        var experienceUnits = gameUnits.GetUnits<UnitExperience>();
        var distanceSqr = info.RangeTakeExperience * info.RangeTakeExperience;
        
        for (int i = experienceUnits.Count - 1; i >= 0; i--)
        {
            if (!experienceUnits[i] 
                || !experienceUnits[i].gameObject.activeInHierarchy
                || !experienceUnits[i].IsTakeExperience
                || !experienceUnits[i].TryGetComponent(out IHealth health)
                || !health.IsLive
                || !Calculate.Distance.IsNearUsingSqr(info.GameObject.transform.position, experienceUnits[i].transform.position, distanceSqr))
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
