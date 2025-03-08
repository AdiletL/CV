using UnityEngine;


namespace Gameplay.Unit.Character.Player
{
    public class PlayerIdleState : CharacterIdleState
    {
        private Vector3 targetPosition;
        
        public override void Subscribe()
        {
            base.Subscribe();
            this.stateMachine.OnExitCategory += OnExitCategory;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            this.stateMachine.OnExitCategory -= OnExitCategory;
        }

        private void OnExitCategory(IState state)
        {
            if(!IsActive) return;
            
            if (typeof(CharacterJumpState).IsAssignableFrom(state.GetType()))
            {
                PlayAnimation();
            }
        }
    }
    

    public class PlayerIdleStateBuilder : CharacterIdleStateBuilder
    {
        public PlayerIdleStateBuilder() : base(new PlayerIdleState())
        {
            
        }
    }
}