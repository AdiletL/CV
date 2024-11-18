namespace Unit.Character.Creep
{
    public class ChestMonsterController : CreepController
    {
        protected override void CreateStates()
        {
            base.CreateStates();

            var animation = components.GetComponentFromArray<ChestMonsterAnimation>();
            
            var idleState = (ChestMonsterIdleState)new ChestMonsterIdleStateBuilder()
                .SetCharacterAnimation(animation)
                .SetIdleClips(soCreepMove.IdleClip)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();
            
            var moveState = (ChestMonsterSwitchMoveState)new ChestMonsterSwitchMoveStateBuilder()
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