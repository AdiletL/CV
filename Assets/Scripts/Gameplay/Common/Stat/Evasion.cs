using UnityEngine;

namespace Gameplay
{
    public class Evasion : IEvasion
    {
        public Stat EvasionStat { get; } = new EvasionStat();
    }

    public class EvasionStat : Stat
    {
        public bool TryEvade()
        {
            return Random.value < CurrentValue;
        }
    }
}