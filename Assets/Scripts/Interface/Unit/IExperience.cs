﻿using Gameplay;

public interface ILevel
{
    public Stat LevelStat { get; }
    public bool IsTakeLevel { get; }

    public void LevelUp(int amount);
    public void IncreaseStats();
}

public interface IExperience
{
    public ICountExperience ICountExperience { get; }
    
    public Stat ExperienceStat { get; }
    
    public bool IsTakeExperience { get; }
    public bool IsGiveExperience { get; }

    public void Initialize();
}
