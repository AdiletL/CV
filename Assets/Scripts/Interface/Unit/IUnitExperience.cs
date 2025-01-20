
using UnityEngine;

public interface IUnitLevel
{
    public int CurrentLevel { get; }
    public bool IsTakeLevel { get; }

    public void LevelUp(int amount);
    public void IncreaseLevel(int value);
}

public interface IUnitExperience
{
    public IExperience ExperienceCalculate { get; }
    
    public int CurrentExperience { get; }
    
    public bool IsTakeExperience { get; }
    public bool IsGiveExperience { get; }

    public void Initialize();

    public void AddExperience(int experience);
}
