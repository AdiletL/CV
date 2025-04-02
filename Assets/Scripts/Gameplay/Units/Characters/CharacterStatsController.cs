using Calculate;
using Gameplay.Resistance;
using ScriptableObjects.Unit.Character;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterStatsController : UnitStatsController, IEvasionApplier
    {
        [SerializeField] protected SO_CharacterStats so_CharacterStats;

        public IEvasion Evasion { get; protected set; }
        
        public override void Initialize()
        {
            base.Initialize();
            var characterMainController = (CharacterMainController)unitController;

            if (TryGetComponent(out IExperience experience))
                AddStatToDictionary(StatType.Experience, experience.ExperienceStat);
            
            if(TryGetComponent(out ILevel level))
                AddStatToDictionary(StatType.Level, level.LevelStat);
            
            if(characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IDamage damage))
                AddStatToDictionary(StatType.Damage, damage.DamageStat);

            if(characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IAttackSpeed attackSpeed))
                AddStatToDictionary(StatType.AttackSpeed, attackSpeed.AttackSpeedStat);
            
            if(characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IRangeAttack rangeAttack))
                AddStatToDictionary(StatType.AttackRange, rangeAttack.RangeAttackStat);
            
            if(characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IMovementSpeed movementSpeed))
                AddStatToDictionary(StatType.MovementSpeed, movementSpeed.MovementSpeedStat);

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
                IPhysicalResistance physicalResistance = new PhysicalDamageResistance(ValueType.Percent, so_CharacterStats.Armor);
                IMagicalResistance magicalResistance = new MagicalDamageResistance(ValueType.Percent, so_CharacterStats.MagicalResistance);
                IPureResistance pureResistance = new PureDamageResistance(ValueType.Percent, so_CharacterStats.PureResistance);
                
                resistanceHandler.AddResistance(physicalResistance);
                resistanceHandler.AddResistance(magicalResistance);
                resistanceHandler.AddResistance(pureResistance);
                
                AddStatToDictionary(physicalResistance.StatTypeID, physicalResistance.ResistanceStat);
                AddStatToDictionary(magicalResistance.StatTypeID, magicalResistance.ResistanceStat);
                AddStatToDictionary(pureResistance.StatTypeID, pureResistance.ResistanceStat);
            }

            Evasion = new Evasion();
            Evasion.EvasionStat.AddCurrentValue(so_CharacterStats.EvasionChance);
            AddStatToDictionary(StatType.Evasion, Evasion.EvasionStat);
        }
    }
}