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
                AddStatToDictionary(UnitStatType.Experience, experience.ExperienceStat);
            
            if(TryGetComponent(out ILevel level))
                AddStatToDictionary(UnitStatType.Level, level.LevelStat);
            
            if(characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IDamage damage))
                AddStatToDictionary(UnitStatType.Damage, damage.DamageStat);

            if(characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IAttackSpeed attackSpeed))
                AddStatToDictionary(UnitStatType.AttackSpeed, attackSpeed.AttackSpeedStat);
            
            if(characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IRangeAttack rangeAttack))
                AddStatToDictionary(UnitStatType.AttackRange, rangeAttack.RangeAttackStat);
            
            if(characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IMovementSpeed movementSpeed))
                AddStatToDictionary(UnitStatType.MovementSpeed, movementSpeed.MovementSpeedStat);

            if (TryGetComponent(out IHealth health))
            {
                AddStatToDictionary(UnitStatType.Health, health.HealthStat);
                AddStatToDictionary(UnitStatType.RegenerationHealth, health.RegenerationStat);
            }

            if (TryGetComponent(out IEndurance endurance))
            {
                AddStatToDictionary(UnitStatType.Endurance, endurance.EnduranceStat);
                AddStatToDictionary(UnitStatType.RegenerationEndurance, endurance.RegenerationStat);
            }

            if (TryGetComponent(out IMana mana))
            {
                AddStatToDictionary(UnitStatType.Mana, mana.ManaStat);
                AddStatToDictionary(UnitStatType.RegenerationMana, mana.RegenerationStat);
            }

            if (TryGetComponent(out ResistanceHandler resistanceHandler))
            {
                IPhysicalResistance physicalResistance = new PhysicalDamageResistance(ValueType.Percent, so_CharacterStats.Armor);
                IMagicalResistance magicalResistance = new MagicalDamageResistance(ValueType.Percent, so_CharacterStats.MagicalResistance);
                IPureResistance pureResistance = new PureDamageResistance(ValueType.Percent, so_CharacterStats.PureResistance);
                
                resistanceHandler.AddResistance(physicalResistance);
                resistanceHandler.AddResistance(magicalResistance);
                resistanceHandler.AddResistance(pureResistance);
                
                AddStatToDictionary(physicalResistance.UnitStatTypeID, physicalResistance.ResistanceStat);
                AddStatToDictionary(magicalResistance.UnitStatTypeID, magicalResistance.ResistanceStat);
                AddStatToDictionary(pureResistance.UnitStatTypeID, pureResistance.ResistanceStat);
            }

            Evasion = new Evasion();
            Evasion.EvasionStat.AddCurrentValue(so_CharacterStats.EvasionChance);
            AddStatToDictionary(UnitStatType.Evasion, Evasion.EvasionStat);
        }
    }
}