using Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderIdleState : CreepIdleState
    { 
        private BeholderSwitchAttackState _beholderSwitchAttackState;
        private BeholderSwitchMoveState _beholderSwitchMoveState;
        
        private float checkEnemyCooldown = .03f;
        private float countCheckEnemyCooldown;
        
        private bool isCheckAttack = true;

        public override void Initialize()
        {
            base.Initialize();
            _beholderSwitchAttackState = (BeholderSwitchAttackState)CharacterSwitchAttack;
            _beholderSwitchMoveState = (BeholderSwitchMoveState)CharacterSwitchMove;
        }
        
        public override void Update()
        {
            base.Update();

            CheckAttack();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            CheckMove();
        }

        private void CheckMove()
        {
            if(!_beholderSwitchMoveState.IsCanMovement() || !isActive) return;
            _beholderSwitchMoveState.ExitCategory(Category);
        }

        private void CheckAttack()
        {
            if(!isCheckAttack || !isActive) return;
            
            countCheckEnemyCooldown += Time.deltaTime;
            if (countCheckEnemyCooldown > checkEnemyCooldown)
            {
                if (_beholderSwitchAttackState.IsFindUnitInRange())
                {
                    _beholderSwitchAttackState.ExitCategory(Category);
                }

                countCheckEnemyCooldown = 0;
            }
        }
    }

    public class BeholderIdleStateBuilder : CreepIdleStateBuilder
    {
        public BeholderIdleStateBuilder() : base(new BeholderIdleState())
        {
        }
        
    }
}