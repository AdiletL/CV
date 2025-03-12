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
            var damageStat = characterMainController.StateMachine.GetState<CharacterAttackState>().DamageStat;
            AddStatToDictionary(StatType.Damage, damageStat);

            var attackSpeedStat = characterMainController.StateMachine.GetState<CharacterAttackState>().AttackSpeedStat;
            AddStatToDictionary(StatType.AttackSpeed, attackSpeedStat);
            
            var rangeAttackStat = characterMainController.StateMachine.GetState<CharacterAttackState>().RangeStat;
            AddStatToDictionary(StatType.AttackRange, rangeAttackStat);
            
            var movementSpeed = characterMainController.StateMachine.GetState<CharacterMoveState>().MovementSpeedStat;
            AddStatToDictionary(StatType.MovementSpeed, movementSpeed);

            if (TryGetComponent(out ResistanceHandler resistanceHandler))
            {
                var damageResistances = resistanceHandler.GetResistances(typeof(DamageResistance));
                DamageResistance damageResistance = null;
                foreach (var VARIABLE in damageResistances)
                {
                    damageResistance = VARIABLE as DamageResistance;
                    if (damageResistance?.DamageType == DamageType.Physical)
                        AddStatToDictionary(StatType.PhysicalResistance, damageResistance.ProtectionStat);
                    else if (damageResistance?.DamageType == DamageType.Magical)
                        AddStatToDictionary(StatType.MagicalResistance, damageResistance.ProtectionStat);
                    else if (damageResistance?.DamageType == DamageType.Pure)
                        AddStatToDictionary(StatType.PureResistance, damageResistance.ProtectionStat);
                }
            }
        }
    }
}