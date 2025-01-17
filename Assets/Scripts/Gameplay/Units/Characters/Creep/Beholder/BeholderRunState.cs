using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderRunState : CreepRunState
    {
        private BeholderSwitchAttack beholderSwitchAttack;

        public override void Initialize()
        {
            base.Initialize();
            beholderSwitchAttack = (BeholderSwitchAttack)CharacterSwitchAttack;
        }

        public override void Update()
        {
            base.Update();
            
            CheckEnemy();
        }

        private void CheckEnemy()
        {
            if (beholderSwitchAttack.IsFindUnitInRange())
                beholderSwitchAttack.ExitCategory(Category);
        }
    }
    
    public class BeholderRunStateBuilder : CreepRunStateBuilder
    {
        public BeholderRunStateBuilder() : base(new BeholderRunState())
        {
        }
    }
}