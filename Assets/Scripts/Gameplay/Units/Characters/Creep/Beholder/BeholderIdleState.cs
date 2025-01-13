using Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderIdleState : CreepIdleState
    { 
        private BeholderSwitchMoveState beholderSwitchMoveState;
        private BeholderSwitchAttackState beholderSwitchAttackState;
        
        private float checkEnemyCooldown = .03f;
        private float countCheckEnemyCooldown;
        
        private bool isCheckAttack = true;
        

        public override void Initialize()
        {
            base.Initialize();
            beholderSwitchMoveState = this.StateMachine.GetState<BeholderSwitchMoveState>();
            beholderSwitchAttackState = this.StateMachine.GetState<BeholderSwitchAttackState>();
        }
        
        public override void Update()
        {
            base.Update();

            CheckAttack();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            CheckPatrol();
        }

        private void CheckPatrol()
        {
            if(beholderSwitchMoveState == null 
               && !beholderSwitchMoveState.IsCanMovement()) return;
            
            this.StateMachine.ExitCategory(Category, typeof(BeholderSwitchMoveState));
        }

        private void CheckAttack()
        {
            if(!isCheckAttack) return;
            
            countCheckEnemyCooldown += Time.deltaTime;
            if (countCheckEnemyCooldown > checkEnemyCooldown)
            {
                if (beholderSwitchAttackState.IsFindUnitInRange())
                {
                    this.StateMachine.ExitCategory(Category, typeof(BeholderSwitchAttackState));
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