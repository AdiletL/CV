using System;
using System.Globalization;
using ValueType = Calculate.ValueType;

namespace Gameplay
{
    public static class UnitStatConvertToString
    {
        public static string Convert(UnitStatType unitStatType, float value, ValueType valueType, int maxScaling = 1, float scalingValue = 0)
        {
            string statValue = null;
            switch (unitStatType)
            {
                case UnitStatType.Nothing:
                    break;
                case UnitStatType.Damage:
                    statValue = "DMG: ";
                    break;
                case UnitStatType.AttackSpeed:
                    statValue = "ATK SPD: ";
                    break;
                case UnitStatType.AttackRange:
                    statValue = "ATK RG: ";
                    break;
                case UnitStatType.MovementSpeed:
                    statValue = "MOVE SPD: ";
                    break;
                case UnitStatType.Health:
                    statValue = "HEALTH: ";
                    break;
                case UnitStatType.RegenerationHealth:
                    statValue = "REGEN HP: ";
                    break;
                case UnitStatType.Mana:
                    statValue = "MANA: ";
                    break;
                case UnitStatType.RegenerationMana:
                    statValue = "REGEN MP: ";
                    break;
                case UnitStatType.Endurance:
                    statValue = "ENDURANCE: ";
                    break;
                case UnitStatType.RegenerationEndurance:
                    statValue = "REGEN ENDURANCE: ";
                    break;
                case UnitStatType.PhysicalDamageResistance:
                    statValue = "PHYS DMG RESIST: ";
                    break;
                case UnitStatType.MagicalDamageResistance:
                    statValue = "MAGIC DMG RESIST: ";
                    break;
                case UnitStatType.PureDamageResistance:
                    statValue = "PURE DMG RESIST: ";
                    break;
                case UnitStatType.Level:
                    statValue = "LVL: ";
                    break;
                case UnitStatType.Experience:
                    statValue = "EXP: ";
                    break;
                case UnitStatType.Evasion:
                    statValue = "EVASION: ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitStatType), unitStatType, null);
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