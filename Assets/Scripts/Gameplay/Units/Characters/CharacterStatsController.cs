using Gameplay.Resistance;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterStatsController : UnitStatsController
    {
        public override void Initialize()
        {
            base.Initialize();
            var characterMainController = (CharacterMainController)unitController;

            if (TryGetComponent(out IExperience experience))
                AddStatToDictionary(StatType.Experience, experience.ExperienceStat);
            
            if(TryGetComponent(out ILevel level))
                AddStatToDictionary(StatType.Level, level.LevelStat);
            
            var damageStat = characterMainController.StateMachine.GetState<CharacterAttackState>()?.DamageStat;
            if(damageStat != null) AddStatToDictionary(StatType.Damage, damageStat);

            var attackSpeedStat = characterMainController.StateMachine.GetState<CharacterAttackState>()?.AttackSpeedStat;
            if(attackSpeedStat != null) AddStatToDictionary(StatType.AttackSpeed, attackSpeedStat);
            
            var rangeAttackStat = characterMainController.StateMachine.GetState<CharacterAttackState>()?.RangeStat;
            if(rangeAttackStat != null) AddStatToDictionary(StatType.AttackRange, rangeAttackStat);
            
            var movementSpeedStat = characterMainController.StateMachine.GetState<CharacterMoveState>()?.MovementSpeedStat;
            if(movementSpeedStat != null) AddStatToDictionary(StatType.MovementSpeed, movementSpeedStat);

            if (TryGetComponent(out IHealth health))
            {
                AddStatToDictionary(StatType.Health, health.HealthStat);
                AddStatToDictionary(StatType.RegenerationHealth, health.RegenerationStat);
            }

            if (TryGetComponent(out IEndurance endurance))
            {
                AddStatToDictionary(StatType.Endurance, endurance.EnduranceStat);
                AddStatToDictionary(StatType.RegenerationEndurance, endurance.RegenerationStat);
            }

            if (TryGetComponent(out IMana mana))
            {
                AddStatToDictionary(StatType.Mana, mana.ManaStat);
                AddStatToDictionary(StatType.RegenerationMana, mana.RegenerationStat);
            }

            if (TryGetComponent(out ResistanceHandler resistanceHandler))
            {
                var damageResistances = resistanceHandler.GetResistances(typeof(DamageResistance));
                DamageResistance damageResistance = null;
                foreach (var VARIABLE in damageResistances)
                {
                    damageResistance = VARIABLE as DamageResistance;
                    if (damageResistance?.DamageType == DamageType.Physical)
                        AddStatToDictionary(StatType.Armor, damageResistance.ProtectionStat);
                    else if (damageResistance?.DamageType == DamageType.Magical)
                        AddStatToDictionary(StatType.MagicalResistance, damageResistance.ProtectionStat);
                    else if (damageResistance?.DamageType == DamageType.Pure)
                        AddStatToDictionary(StatType.PureResistance, damageResistance.ProtectionStat);
                }
            }
        }
    }
}