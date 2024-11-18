using ScriptableObjects.Unit.Character.Creep;

namespace Unit.Character.Creep
{
    public class ChestMonsterSwitchMoveState : CreepSwitchMoveState
    {
        private SO_ChestMonsterMove so_HedgehogMove;
        private ChestMonsterAnimation hedgehogAnimation;
        
        public Platform StartPlatform { get; set; }
        public Platform EndPlatform { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            so_HedgehogMove = (SO_ChestMonsterMove)SO_CharacterMove;
            hedgehogAnimation = (ChestMonsterAnimation)CharacterAnimation;
        }

        protected override void DestermineState()
        {
            base.DestermineState();
            if (!movementStates.ContainsKey(typeof(ChestMonsterPatrolState)))
            {
                var patrolState = (ChestMonsterPatrolState)new ChestMonsterPatrolStateBuilder()
                    .SetEnemyAnimation(hedgehogAnimation)
                    .SetWalkClip(so_HedgehogMove.WalkClip)
                    .SetStartPoint(StartPlatform)
                    .SetEndPoint(EndPlatform)
                    .SetGameObject(GameObject)
                    .SetMovementSpeed(so_HedgehogMove.RunSpeed)
                    .SetStateMachine(this.StateMachine)
                    .Build();
                
                patrolState.Initialize();
                movementStates.TryAdd(typeof(ChestMonsterPatrolState), patrolState);
                this.StateMachine.AddStates(patrolState);
            }
            
            this.StateMachine.SetStates(typeof(ChestMonsterPatrolState));
        }
    }
    public class ChestMonsterSwitchMoveStateBuilder : CreepSwitchMoveStateBuilder
    {
        public ChestMonsterSwitchMoveStateBuilder() : base(new ChestMonsterSwitchMoveState())
        {
        }
        public ChestMonsterSwitchMoveStateBuilder SetStartPlatform(Platform startPlatform)
        {
            if (state is ChestMonsterSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.StartPlatform = startPlatform;
            }

            return this;
        }
        public ChestMonsterSwitchMoveStateBuilder SetEndPlatform(Platform endPlatform)
        {
            if (state is ChestMonsterSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.EndPlatform = endPlatform;
            }

            return this;
        }
    }
}