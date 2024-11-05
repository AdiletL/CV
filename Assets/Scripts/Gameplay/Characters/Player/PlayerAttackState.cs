using UnityEngine;

namespace Character.Player
{
    public class PlayerAttackState : CharacterAttackState
    {
        
        protected override void DestermineState()
        {
            var playerMeleeAttackState = this.StateMachine.GetState<PlayerMeleeAttackState>();
            if(playerMeleeAttackState == null) return;
            
            if (curretAttackState != playerMeleeAttackState)
            {
                curretAttackState = playerMeleeAttackState;
                curretAttackState.Initialize();
            }
            
            this.StateMachine.SetStates(typeof(PlayerMeleeAttackState));
        }

    }

    public class PlayerAttackStateBuilder : CharacterAttackStateBuilder
    {
        public PlayerAttackStateBuilder() : base(new PlayerAttackState())
        {
        }
        
    }
}