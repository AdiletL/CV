using UnityEngine;

namespace Character.Enemy
{
    public class EnemyIdleState : CharacterIdleState
    {
        public override void Update()
        {
            base.Update();
            this.StateMachine.ExitCategory(Category);
            this.StateMachine.SetStates(typeof(EnemyMoveState));
        }
    }

    public class EnemyIdleStateBuilder : CharacterIdleStateBuilder
    {
        public EnemyIdleStateBuilder(CharacterIdleState instance) : base(instance)
        {
        }
        
        
    }
}