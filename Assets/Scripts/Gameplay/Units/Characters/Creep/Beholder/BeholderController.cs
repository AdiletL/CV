using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderController : CreepController
    {
        [SerializeField] private Transform finalPath;
        [SerializeField] private SO_BeholderAttack so_BeholderAttack;
        
        
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
                .SetCharacterController(GetComponent<CharacterController>())
                .SetStart(Calculate.FindCell.GetCell(transform.position, Vector3.down).transform)
                .SetEnd(finalPath)
                .SetEnemyLayer(so_BeholderAttack.EnemyLayer)
                .SetCenter(center)
                .SetCharacterAnimation(animation)
                .SetConfig(soCreepMove)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();

            var attackState = (BeholderSwitchAttackState)new BeholderSwitchAttackStateBuilder()
                .SetCenter(center)
                .SetCharacterAnimation(animation)
                .SetEnemyLayer(so_BeholderAttack.EnemyLayer)
                .SetGameObject(gameObject)
                .SetConfig(so_BeholderAttack)
                .SetStateMachine(this.StateMachine)
                .Build();

            this.StateMachine.AddStates(idleState, moveState, attackState);
        }

        public override void Appear()
        {
            Debug.Log("Appeared");
            this.StateMachine.SetStates(typeof(BeholderIdleState));
        }
    }
}