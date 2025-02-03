using Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderIdleState : CreepIdleState
    { 
        private BeholderSwitchAttackState _beholderSwitchAttackState;
        private CreepSwitchMoveState creepSwitchMoveState;
        
        private float checkEnemyCooldown = .03f;
        private float countCheckEnemyCooldown;
        
        private bool isCheckAttack = true;
        
        public void SetCreepSwitchMoveState(CreepSwitchMoveState creepSwitchMoveState) => this.creepSwitchMoveState = creepSwitchMoveState;
        
        
        public override void Update()
        {
            base.Update();

            //CheckAttack();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            CheckMove();
        }

        private void CheckMove()
        {
            if(!creepSwitchMoveState.IsCanMovement() || !isActive) return;
            creepSwitchMoveState.ExitCategory(Category);
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

        public BeholderIdleStateBuilder SetCreepSwitchMoveState(CreepSwitchMoveState creepSwitchMoveState)
        {
            if(state is BeholderIdleState beholderIdleState)
                beholderIdleState.SetCreepSwitchMoveState(creepSwitchMoveState);
            return this;
        }
    }
}