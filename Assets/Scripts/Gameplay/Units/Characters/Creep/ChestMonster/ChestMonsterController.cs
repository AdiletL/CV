using UnityEngine;
using UnityEngine.Serialization;

namespace Unit.Character.Creep
{
    public class ChestMonsterController : CreepController
    {
        [SerializeField] private Transform finalPath;
        
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