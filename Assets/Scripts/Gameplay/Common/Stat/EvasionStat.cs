using UnityEngine;

namespace Gameplay
{
    public class EvasionStat : Stat, IEvasion
    {
        public bool TryEvade()
        {
            return Random.value < CurrentValue;
        }
    }
}