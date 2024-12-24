namespace Unit.Character.Creep
{
    public class ChestMonsterIdleState : CreepIdleState
    {
        private int checkEnemyLayer { get; } = Layers.PLAYER_LAYER;
        
        private ChestMonsterSwitchMoveState chestMonsterSwitchMoveState;
        private ChestMonsterSwitchAttackState chestMonsterSwitchAttackState;
        
        private float checkEnemyCooldown = .03f;
        private float countCheckEnemyCooldown;
        
        private bool isCheckAttack;

        public override void Initialize()
        {
            base.Initialize();
            chestMonsterSwitchMoveState = this.StateMachine.GetState<ChestMonsterSwitchMoveState>();
            //chestMonsterSwitchAttackState = this.StateMachine.GetState<ChestMonsterSwitchAttackState>();
        }


        public override void Update()
        {
            base.Update();

            //CheckAttack();
            CheckPatrol();
        }

        private void CheckPatrol()
        {
            if(chestMonsterSwitchMoveState == null 
               && !chestMonsterSwitchMoveState.IsCanMovement()) return;
            
            this.StateMachine.ExitCategory(Category);
            this.StateMachine.SetStates(typeof(ChestMonsterSwitchMoveState));
        }
    }

    public class ChestMonsterIdleStateBuilder : CreepIdleStateBuilder
    {
        public ChestMonsterIdleStateBuilder() : base(new ChestMonsterIdleState())
        {
        }
    }
}