using Calculate;
using UnityEngine;

namespace Gameplay
{
    public class CriticalDamage : ICriticalDamage
    {
        public Stat CriticalDamageStat { get; } = new Stat();
        
        private System.Random random = new System.Random();
        private float accumulatedChance;

        public float GetCalculateDamage(float baseDamage)
        {
            var gameValue = new GameValue(CriticalDamageStat.CurrentValue, ValueType.Percent);
            return gameValue.Calculate(baseDamage);
        }
        
        public bool TryApply()
        {
            if (CriticalDamageStat.CurrentValue == 0) return false;
            float currentChance = CriticalDamageStat.CurrentValue + accumulatedChance;

            // Псевдослучайная проверка
            if (random.NextDouble() < currentChance)
            {
                accumulatedChance = 0f;
                return true;
            }

            accumulatedChance += CriticalDamageStat.CurrentValue * (1f - currentChance); // При промахе увеличиваем шанс
            accumulatedChance = Mathf.Min(accumulatedChance, 1f); // Защита от переполнения

            return false;
        }
    }
}