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
            
            var damageStat = characterMainController.StateMachine.GetState<CharacterAttackState>()?.DamageStat;
            if(damageStat != null) AddStatToDictionary(StatType.Damage, damageStat);

            var attackSpeedStat = characterMainController.StateMachine.GetState<CharacterAttackState>()?.AttackSpeedStat;
            if(attackSpeedStat != null) AddStatToDictionary(StatType.AttackSpeed, attackSpeedStat);
            
            var rangeAttackStat = characterMainController.StateMachine.GetState<CharacterAttackState>()?.RangeStat;
            if(rangeAttackStat != null) AddStatToDictionary(StatType.AttackRange, rangeAttackStat);
            
            var movementSpeedStat = characterMainController.StateMachine.GetState<CharacterMoveState>()?.MovementSpeedStat;
            if(movementSpeedStat != null) AddStatToDictionary(StatType.MovementSpeed, movementSpeedStat);
            
            var healthStat = GetComponent<CharacterHealth>()?.HealthStat;
            if(healthStat != null) AddStatToDictionary(StatType.Health, healthStat);
            
            var enduranceStat = GetComponent<CharacterEndurance>()?.EnduranceStat;
            if(enduranceStat != null) AddStatToDictionary(StatType.Endurance, enduranceStat);

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