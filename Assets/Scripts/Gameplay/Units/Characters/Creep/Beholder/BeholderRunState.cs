using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderRunState : CreepRunState
    {
        private BeholderSwitchAttackState beholderSwitchAttackState;

        public override void Initialize()
        {
            base.Initialize();
            beholderSwitchAttackState = this.StateMachine.GetState<BeholderSwitchAttackState>();
        }

        public override void Update()
        {
            base.Update();
            
            CheckEnemy();
        }

        private void CheckEnemy()
        {
            if (beholderSwitchAttackState.IsFindUnitInRange())
                this.StateMachine.ExitCategory(Category, typeof(BeholderSwitchAttackState));
        }
    }
    
    public class BeholderRunStateBuilder : CreepRunStateBuilder
    {
        public BeholderRunStateBuilder() : base(new BeholderRunState())
        {
        }
    }
}