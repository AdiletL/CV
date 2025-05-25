using System;
using Gameplay;

public interface IStatsController<T> where T : Enum
{
    public Stat GetStat(T statType);
}