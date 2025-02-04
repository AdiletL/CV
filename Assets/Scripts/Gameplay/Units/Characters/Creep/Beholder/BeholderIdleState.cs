using Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderIdleState : CreepIdleState
    { 
        private CreepSwitchAttackState creepSwitchAttackState;
        private CreepSwitchMoveState creepSwitchMoveState;
        
        private float countCheckEnemyCooldown;
        private const float checkEnemyCooldown = .03f;
        
        public void SetCreepSwitchAttackState(CreepSwitchAttackState creepSwitchAttackState) => this.creepSwitchAttackState = creepSwitchAttackState;
        public void SetCreepSwitchMoveState(CreepSwitchMoveState creepSwitchMoveState) => this.creepSwitchMoveState = creepSwitchMoveState;
        
        
        public override void Update()
        {
            base.Update();

            CheckEnemy();
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

        private void CheckEnemy()
        {
            if(!isActive) return;
            
            countCheckEnemyCooldown += Time.deltaTime;
            if (countCheckEnemyCooldown > checkEnemyCooldown)
            {
                if (creepSwitchAttackState.IsFindUnitInRange())
                    creepSwitchAttackState.ExitCategory(Category);

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
        
        public BeholderIdleStateBuilder SetCreepSwitchAttackState(CreepSwitchAttackState creepSwitchAttackState)
        {
            if(state is BeholderIdleState beholderIdleState)
                beholderIdleState.SetCreepSwitchAttackState(creepSwitchAttackState);
            return this;
        }
    }
}