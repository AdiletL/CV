namespace Unit.Character.Creep
{
    public class BeholderController : CreepController
    {
        protected override void CreateStates()
        {
            var animation = components.GetComponentFromArray<BeholderAnimation>();
            
            var idleState = (BeholderIdleState)new BeholderIdleStateBuilder()
                .SetCharacterAnimation(animation)
                .SetIdleClips(soCreepMove.IdleClip)
                .SetCenter(center)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();
            
            var moveState = (BeholderSwitchMoveState)new BeholderSwitchMoveStateBuilder()
                .SetStartPlatform(startPlatform)
                .SetEndPlatform(endPlatform)
                .SetCenter(center)
                .SetCharacterAnimation(animation)
                .SetConfig(soCreepMove)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();

            this.StateMachine.AddStates(idleState, moveState);
        }

    }
}