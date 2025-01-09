using UnityEngine;

namespace Unit.Character.Creep
{
    public class HedgehogIdleState : CreepIdleState
    {
        private int checkEnemyLayer { get; } = Layers.PLAYER_LAYER;
        
        private HedgehogSwitchMoveState hedgehogSwitchMoveState;
        private HedgehogSwitchAttackState hedgehogSwitchAttackState;
        
        private float checkEnemyCooldown = .03f;
        private float countCheckEnemyCooldown;
        
        private bool isCheckAttack;

        public override void Initialize()
        {
            base.Initialize();
            hedgehogSwitchMoveState = this.StateMachine.GetState<HedgehogSwitchMoveState>();
            //hedgehogSwitchAttackState = this.StateMachine.GetState<HedgehogSwitchAttackState>();
        }


        public override void Update()
        {
            base.Update();

            //CheckAttack();
            CheckPatrol();
        }

        private void CheckPatrol()
        {
            if(hedgehogSwitchMoveState == null 
               && !hedgehogSwitchMoveState.IsCanMovement()) return;
            
            this.StateMachine.ExitCategory(Category, typeof(HedgehogSwitchMoveState));
        }
    }

    public class HedgehogIdleStateBuilder : CreepIdleStateBuilder
    {
        public HedgehogIdleStateBuilder() : base(new HedgehogIdleState())
        {
        }
    }
}