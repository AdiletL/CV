using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderPatrolState : CreepPatrolState
    {
        private BeholderSwitchAttackState beholderSwitchAttackState;

        public override void Initialize()
        {
            base.Initialize();
            beholderSwitchAttackState = this.StateMachine.GetState<BeholderSwitchAttackState>();
        }

        public override void Enter()
        {
            base.Enter();
            CheckEnemy();
        }

        public override void LateUpdate()
        {
            CheckEnemy();
        }

        private void CheckEnemy()
        {
            if (beholderSwitchAttackState.IsFindUnitInRange())
                this.StateMachine.ExitCategory(Category, typeof(BeholderSwitchAttackState));
        }
    }
    
    public class BeholderPatrolStateBuilder : CreepPatrolStateBuilder
    {
        public BeholderPatrolStateBuilder() : base(new BeholderPatrolState())
        {
        }
    }
}