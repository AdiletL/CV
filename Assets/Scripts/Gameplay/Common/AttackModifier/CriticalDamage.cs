using Calculate;
using UnityEngine;

namespace Gameplay.AttackModifier
{
    public class CriticalDamage : AttackModifier, ICriticalDamage
    {
        public override AttackModifierType AttackModifierTypeID { get; } = AttackModifierType.CriticalDamage;
        
        private System.Random random = new System.Random();
        private float accumulatedChance;
        
        public ValueType ValueTypeID { get; }
        public float Value { get; }
        public float ChanceValue { get; }
        public bool IsActive { get; protected set; } = true;

        public CriticalDamage(float value, ValueType valueType, float chanceValue)
        {
            ValueTypeID = valueType;
            Value = value;
            ChanceValue = chanceValue;
        }

        public float GetCalculateDamage(float baseDamage)
        {
            var gameValue = new GameValue(Value, ValueTypeID);
            return gameValue.Calculate(baseDamage);
        }
        
        public bool TryApply()
        {
            if (Value == 0 || !IsActive) return false;
            float currentChance = ChanceValue + accumulatedChance;

            // Псевдослучайная проверка
            if (random.NextDouble() < currentChance)
            {
                accumulatedChance = 0f;
                return true;
            }

            accumulatedChance += ChanceValue * (1f - currentChance); // При промахе увеличиваем шанс
            accumulatedChance = Mathf.Min(accumulatedChance, 1f); // Защита от переполнения

            return false;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }

    [System.Serializable]
    public class CriticalDamageConfig
    {
        public GameValueConfig GameValueConfig;
        [Range(0f, 1f)]
        public float Chance;
    }
}