using Gameplay;

public interface ILevel
{
    public Stat LevelStat { get; }
    public bool IsTakeLevel { get; }

    public void LevelUp(int amount);
}

public interface IExperience
{
    public ICountExperience ICountExperienceCalculate { get; }
    
    public Stat ExperienceStat { get; }
    
    public bool IsTakeExperience { get; }
    public bool IsGiveExperience { get; }

    public void Initialize();
}
