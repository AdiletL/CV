using ScriptableObjects.Unit.Character.Creep;

namespace Unit.Character.Creep
{
    public class BeholderSwitchMoveState : CreepSwitchMoveState
    {
        private SO_BeholderMove so_HedgehogMove;
        private BeholderAnimation hedgehogAnimation;
        
        public Platform StartPlatform { get; set; }
        public Platform EndPlatform { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            so_HedgehogMove = (SO_BeholderMove)SO_CharacterMove;
            hedgehogAnimation = (BeholderAnimation)CharacterAnimation;
        }

        protected override void DestermineState()
        {
            base.DestermineState();
            if (!movementStates.ContainsKey(typeof(BeholderPatrolState)))
            {
                var patrolState = (BeholderPatrolState)new BeholderPatrolStateBuilder()
                    .SetEnemyAnimation(hedgehogAnimation)
                    .SetWalkClip(so_HedgehogMove.WalkClip)
                    .SetStartPoint(StartPlatform)
                    .SetEndPoint(EndPlatform)
                    .SetGameObject(GameObject)
                    .SetMovementSpeed(so_HedgehogMove.RunSpeed)
                    .SetStateMachine(this.StateMachine)
                    .Build();
                
                patrolState.Initialize();
                movementStates.TryAdd(typeof(BeholderPatrolState), patrolState);
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