using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderPatrolState : CreepPatrolState
    {
        private BeholderSwitchAttack beholderSwitchAttack;

        public override void Initialize()
        {
            base.Initialize();
            beholderSwitchAttack = (BeholderSwitchAttack)CharacterSwitchAttack;
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
            if (beholderSwitchAttack.IsFindUnitInRange())
                beholderSwitchAttack.ExitCategory(Category);
        }
    }
    
    public class BeholderPatrolStateBuilder : CreepPatrolStateBuilder
    {
        public BeholderPatrolStateBuilder() : base(new BeholderPatrolState())
        {
        }
    }
}