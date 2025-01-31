using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderPatrolState : CreepPatrolState
    {
        private BeholderSwitchAttackState _beholderSwitchAttackState;

        public override void Initialize()
        {
            base.Initialize();
            _beholderSwitchAttackState = (BeholderSwitchAttackState)CharacterSwitchAttack;
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
            if (_beholderSwitchAttackState.IsFindUnitInRange())
            {
                _beholderSwitchAttackState.ExitCategory(Category);
            }
        }
    }
    
    public class BeholderPatrolStateBuilder : CreepPatrolStateBuilder
    {
        public BeholderPatrolStateBuilder() : base(new BeholderPatrolState())
        {
        }
    }
}