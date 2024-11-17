using UnityEngine;

namespace Unit.Character.Creep
{
    public class HedgehogController : CreepController
    {
        public override UnitType UnitType { get; } = UnitType.creep;

        
        protected override void CreateStates()
        {
            base.CreateStates();

            var animation = components.GetComponentFromArray<HedgehogAnimation>();
            
            var idleState = (HedgehogIdleIdleState)new HedgehogIdleStateBuilder()
                .SetCharacterAnimation(animation)
                .SetIdleClips(soCreepMove.IdleClip)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();;
            
            var moveState = (HedgehogSwitchMoveState)new HedgehogMoveStateBuilder()
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