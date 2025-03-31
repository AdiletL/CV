using Calculate;
using Gameplay.Resistance;
using ScriptableObjects.Unit.Character;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterStatsController : UnitStatsController
    {
        [SerializeField] protected SO_CharacterStats so_CharacterStats;
        
        protected Stat evasionStat = new EvasionStat();
        
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
            
            var rangeAttackStat = characterMainController.StateMachine.GetState<CharacterAttackState>()?.RangeAttackStat;
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
                var armor = new DamageResistance(StatType.Armor, ValueType.Percent, so_CharacterStats.Armor);
                var magicalResistance = new DamageResistance(StatType.MagicalResistance, ValueType.Percent, so_CharacterStats.MagicalResistance);
                var pureResistance = new DamageResistance(StatType.PureResistance, ValueType.Percent, so_CharacterStats.PureResistance);
                
                resistanceHandler.AddResistance(armor);
                resistanceHandler.AddResistance(magicalResistance);
                resistanceHandler.AddResistance(pureResistance);
                
                AddStatToDictionary(armor.StatTypeID, armor.ProtectionStat);
                AddStatToDictionary(magicalResistance.StatTypeID, magicalResistance.ProtectionStat);
                AddStatToDictionary(pureResistance.StatTypeID, pureResistance.ProtectionStat);
            }

            evasionStat = new EvasionStat();
            evasionStat.AddCurrentValue(so_CharacterStats.EvasionChance);
            AddStatToDictionary(StatType.Evasion, evasionStat);
        }
    }
}