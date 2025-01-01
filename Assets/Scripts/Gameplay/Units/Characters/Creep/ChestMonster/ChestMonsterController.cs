namespace Unit.Character.Creep
{
    public class ChestMonsterController : CreepController
    {
        protected override void CreateStates()
        {
            var animation = components.GetComponentFromArray<ChestMonsterAnimation>();
            
            var idleState = (ChestMonsterIdleState)new ChestMonsterIdleStateBuilder()
                .SetCharacterAnimation(animation)
                .SetIdleClips(soCreepMove.IdleClip)
                .SetCenter(center)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();
            
            var moveState = (ChestMonsterSwitchMoveState)new ChestMonsterSwitchMoveStateBuilder()
                .SetStart(start)
                .SetEnd(end)
                .SetEnemyLayer(Layers.PLAYER_LAYER)
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