using Gameplay.Resistance;
using UnityEngine;

namespace Gameplay.Unit.Character.Player
{
    public class PlayerStatsController : CharacterStatsController
    {
        private PlayerController playerController;
        
        public override void Initialize()
        {
            base.Initialize();
            playerController = (PlayerController)unitController;
            var damageStat = playerController.StateMachine.GetState<PlayerAttackState>().DamageStat;
            AddStatToDictionary(StatType.Damage, damageStat);

            var attackSpeedStat = playerController.StateMachine.GetState<PlayerAttackState>().AttackSpeedStat;
            AddStatToDictionary(StatType.AttackSpeed, attackSpeedStat);
            
            var rangeAttackStat = playerController.StateMachine.GetState<PlayerAttackState>().RangeStat;
            AddStatToDictionary(StatType.AttackRange, rangeAttackStat);
            
            var movementSpeed = playerController.StateMachine.GetState<PlayerMoveState>().MovementSpeedStat;
            AddStatToDictionary(StatType.MovementSpeed, movementSpeed);

            var resistanceHandler = GetComponent<ResistanceHandler>();
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