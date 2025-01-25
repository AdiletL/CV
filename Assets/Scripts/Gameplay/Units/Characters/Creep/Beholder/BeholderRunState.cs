using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderRunState : CreepRunState
    {
        private BeholderSwitchAttack _beholderSwitchAttack;

        public override void Initialize()
        {
            base.Initialize();
            _beholderSwitchAttack = (BeholderSwitchAttack)CharacterSwitchAttack;
        }

        public override void Update()
        {
            base.Update();
            
            CheckEnemy();
        }

        private void CheckEnemy()
        {
            if (_beholderSwitchAttack.IsFindUnitInRange())
                _beholderSwitchAttack.ExitCategory(Category);
        }
    }
    
    public class BeholderRunStateBuilder : CreepRunStateBuilder
    {
        public BeholderRunStateBuilder() : base(new BeholderRunState())
        {
        }
    }
}