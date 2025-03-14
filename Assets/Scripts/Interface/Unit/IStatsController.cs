using System.Collections.Generic;
using Gameplay;

public interface IStatsController
{
    public Dictionary<StatType, Stat> Stats { get; }
    public Stat GetStat(StatType statType);
    public void Initialize();
}
