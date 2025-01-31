using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderRunState : CreepRunState
    {
        private BeholderSwitchAttackState _beholderSwitchAttackState;

        public override void Initialize()
        {
            base.Initialize();
            _beholderSwitchAttackState = (BeholderSwitchAttackState)characterSwitchAttackState;
        }

        public override void Update()
        {
            base.Update();
            
            CheckEnemy();
        }

        private void CheckEnemy()
        {
            if (_beholderSwitchAttackState.IsFindUnitInRange())
                _beholderSwitchAttackState.ExitCategory(Category);
        }
    }
    
    public class BeholderRunStateBuilder : CreepRunStateBuilder
    {
        public BeholderRunStateBuilder() : base(new BeholderRunState())
        {
        }
    }
}