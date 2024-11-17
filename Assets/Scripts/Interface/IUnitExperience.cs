
using UnityEngine;

public interface IUnitLevel
{
    public int CurrentLevel { get; }
    public bool IsTakeLevel { get; }

    public void LevelUp(int amount);
    public void IncreaseLevel(Unit.IState states);
}

public interface IUnitExperience
{
    public IExperience ExperienceCalculate { get; }
    
    public int CurrentExperience { get; }
    public int GiveExperience { get; }
    
    public bool IsTakeExperience { get; }
    public bool IsGiveExperience { get; }

    public bool IsRangeTakeExperience(GameObject target);
    
    public void Initialize();

    public void AddExperience(int experience);
}
