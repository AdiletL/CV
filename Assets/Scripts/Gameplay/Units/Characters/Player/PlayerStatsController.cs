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
            var damageStat = playerController.StateMachine.GetState<PlayerAttackState>().DamageStat;
            AddStatToDictionary(StatType.Damage, damageStat);

            var attackSpeedStat = playerController.StateMachine.GetState<PlayerAttackState>().AttackSpeedStat;
            AddStatToDictionary(StatType.AttackSpeed, attackSpeedStat);
            
            var rangeAttackStat = playerController.StateMachine.GetState<PlayerAttackState>().RangeStat;
            AddStatToDictionary(StatType.AttackRange, rangeAttackStat);
            
            var movementSpeed = playerController.StateMachine.GetState<PlayerMoveState>().MovementSpeedStat;
            AddStatToDictionary(StatType.MovementSpeed, movementSpeed);
        }
    }
}