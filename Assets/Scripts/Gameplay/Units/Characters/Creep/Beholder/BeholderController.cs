﻿namespace Unit.Character.Creep
{
    public class BeholderController : CreepController
    {
        protected override void CreateStates()
        {
            base.CreateStates();

            var animation = components.GetComponentFromArray<BeholderAnimation>();
            
            var idleState = (BeholderIdleState)new BeholderIdleStateBuilder()
                .SetCharacterAnimation(animation)
                .SetIdleClips(soCreepMove.IdleClip)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();
            
            var moveState = (BeholderSwitchMoveState)new BeholderSwitchMoveStateBuilder()
                .SetStartPlatform(startPlatform)
                .SetEndPlatform(endPlatform)
                .SetCharacterAnimation(animation)
                .SetConfig(soCreepMove)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();

            this.StateMachine.AddStates(idleState, moveState);
        }

    }
}