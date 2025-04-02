using UnityEngine;

namespace Gameplay
{
    public class Evasion : IEvasion
    {
        public Stat EvasionStat { get; } = new Stat();
        
        private float accumulatedChance;
        private System.Random random = new System.Random();
        
        public bool TryEvade()
        {
            if (EvasionStat.CurrentValue == 0) return false;
            float currentChance = EvasionStat.CurrentValue + accumulatedChance;

            // Псевдослучайная проверка
            if (random.NextDouble() < currentChance)
            {
                accumulatedChance = 0f;
                return true;
            }

            accumulatedChance += EvasionStat.CurrentValue * (1f - currentChance); // При промахе увеличиваем шанс
            accumulatedChance = Mathf.Min(accumulatedChance, 1f); // Защита от переполнения

            return false;
        }
    }
}