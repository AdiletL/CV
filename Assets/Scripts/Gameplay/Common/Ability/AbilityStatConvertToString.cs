using System;
using System.Globalization;
using ValueType = Calculate.ValueType;

namespace Gameplay.Ability
{
    public static class AbilityStatConvertToString
    {
        public static string Convert(AbilityStatType abilityStatType, float value, ValueType valueType,
            int maxScaling = 1, float scalingValue = 0)
        {
            string statValue = null;
            switch (abilityStatType)
            {
                case AbilityStatType.Nothing:
                    break;
                case AbilityStatType.Damage:
                    statValue = $"DMG: ";
                    break;
                case AbilityStatType.Cooldown:
                    statValue = $"Cooldown: ";
                    break;
                case AbilityStatType.Range:
                    statValue = $"Range: ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityStatType), abilityStatType, null);
            }

            for (int i = 0; i < maxScaling; i++)
            {
                float currentValue = value + (i * scalingValue);

                // Проверяем: есть ли дробная часть
                if (currentValue % 1 == 0)
                    statValue += currentValue.ToString("0");
                else
                    statValue += currentValue.ToString("0.0", CultureInfo.InvariantCulture);

                switch (valueType)
                {
                    case ValueType.Fixed: break;
                    case ValueType.Percent: statValue += "%"; break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null);
                }

                if (scalingValue == 0) break;

                if (i != maxScaling - 1)
                    statValue += "/";
            }

            return statValue;
        }
    }
}