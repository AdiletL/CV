using UnityEngine;
using UnityEngine.Serialization;

namespace Unit.Character.Creep
{
    public class HedgehogController : CreepController
    {
        [SerializeField] private Transform finalPath;
        
        protected override void CreateStates()
        {
            var animation = components.GetComponentFromArray<HedgehogAnimation>();
            
            var idleState = (HedgehogIdleState)new HedgehogIdleStateBuilder()
                .SetCharacterAnimation(animation)
                .SetIdleClips(soCreepMove.IdleClip)
                .SetCenter(center)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();;
            
            var moveState = (HedgehogSwitchMoveState)new HedgehogSwitchMoveStateBuilder()
                .SetStart(gameObject.transform)
                .SetEnd(finalPath)
                .SetEnemyLayer(Layers.PLAYER_LAYER)
                .SetCenter(center)
                .SetCharacterAnimation(animation)
                .SetConfig(soCreepMove)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();

            this.StateMachine.AddStates(idleState, moveState);
        }

        
        public override void Appear()
        {
            
        }
    }
}