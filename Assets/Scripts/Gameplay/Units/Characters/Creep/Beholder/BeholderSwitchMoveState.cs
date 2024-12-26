using ScriptableObjects.Unit.Character.Creep;

namespace Unit.Character.Creep
{
    public class BeholderSwitchMoveState : CreepSwitchMoveState
    {
        private SO_BeholderMove so_BeholderMove;
        private BeholderAnimation beholderAnimation;
        
        public Platform StartPlatform { get; set; }
        public Platform EndPlatform { get; set; }

        public override bool IsCanMovement()
        {
            return StartPlatform && EndPlatform;
        }

        private BeholderPatrolState CreatePatrolState()
        {
            return (BeholderPatrolState)new BeholderPatrolStateBuilder()
                .SetCenter(Center)
                .SetEnemyAnimation(beholderAnimation)
                .SetWalkClip(so_BeholderMove.WalkClip)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetStartPoint(StartPlatform)
                .SetEndPoint(EndPlatform)
                .SetGameObject(GameObject)
                .SetMovementSpeed(so_BeholderMove.RunSpeed)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        public override void Initialize()
        {
            base.Initialize();
            so_BeholderMove = (SO_BeholderMove)SO_CharacterMove;
            beholderAnimation = (BeholderAnimation)CharacterAnimation;
        }

        protected override void DestermineState()
        {
            base.DestermineState();
            if (!this.StateMachine.IsStateNotNull(typeof(BeholderPatrolState)))
            {
                var patrolState = CreatePatrolState();
                
                patrolState.Initialize();
                this.StateMachine.AddStates(patrolState);
            }
            
            this.StateMachine.SetStates(typeof(BeholderPatrolState));
        }
    }
    
    public class BeholderSwitchMoveStateBuilder : CreepSwitchMoveStateBuilder
    {
        public BeholderSwitchMoveStateBuilder() : base(new BeholderSwitchMoveState())
        {
        }
        
        public BeholderSwitchMoveStateBuilder SetStartPlatform(Platform startPlatform)
        {
            if (state is BeholderSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.StartPlatform = startPlatform;
            }

            return this;
        }
        public BeholderSwitchMoveStateBuilder SetEndPlatform(Platform endPlatform)
        {
            if (state is BeholderSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.EndPlatform = endPlatform;
            }

            return this;
        }
    }
}