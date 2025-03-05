using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerStatsController : CharacterStatsController
    {
        private PlayerController playerController;
        
        public override void Initialize()
        {
            base.Initialize();
            playerController = (PlayerController)unitController;
            var damageStat = playerController.StateMachine.GetState<PlayerWeaponAttackState>().DamageStat;
            AddStatToDictionary(StatType.Damage, damageStat);

            var attackSpeedStat = playerController.StateMachine.GetState<PlayerWeaponAttackState>().AttackSpeedStat;
            AddStatToDictionary(StatType.AttackSpeed, attackSpeedStat);
            
            var rangeAttackStat = playerController.StateMachine.GetState<PlayerWeaponAttackState>().RangeStat;
            AddStatToDictionary(StatType.AttackRange, rangeAttackStat);
            
            var movementSpeed = playerController.StateMachine.GetState<PlayerRunState>().MovementSpeedStat;
            AddStatToDictionary(StatType.MovementSpeed, movementSpeed);
        }
    }
}