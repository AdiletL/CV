using Machine;
using ScriptableObjects.Unit.Character.Creep;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unit.Character.Creep
{
    public abstract class CreepController : CharacterMainController
    {
        public override UnitType UnitType { get; } = UnitType.creep;

        [FormerlySerializedAs("so_EnemyMove")] [SerializeField] protected SO_CreepMove soCreepMove;
        
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;
        
        protected Platform startPlatform;
        protected Platform endPlatform;
        
        public override void Initialize()
        {
            base.Initialize();

            CreateStates();
            
            this.StateMachine.Initialize();
            components.GetComponentFromArray<CreepHealth>()?.Initialize();
            
            this.StateMachine.SetStates(typeof(CreepIdleState));
            
            this.StateMachine.OnChangedState += OnChangedState;
        }

        protected virtual void CreateStates()
        {
            this.StateMachine = new StateMachine();
            
            var animation = components.GetComponentFromArray<CreepAnimation>();

            var idleState = (CreepIdleState)new CreepIdleStateBuilder(new CreepIdleState())
                .SetCharacterAnimation(animation)
                .SetIdleClips(soCreepMove.IdleClip)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();;
            
            var attackState = (CreepSwitchAttackState)new CreepSwitchAttackStateBuilder(new CreepSwitchAttackState())
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();

            var moveState = (CreepSwitchMoveState)new CreepSwitchMoveStateBuilder(new CreepSwitchMoveState())
                .SetConfig(soCreepMove)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();

            var patrolState = (CreepPatrolState)new CreepPatrolStateBuilder(new CreepPatrolState())
                .SetMovementSpeed(soCreepMove.WalkSpeed)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();
            
            this.StateMachine.AddStates(idleState, moveState);
        }
        
                
        public void SetStartPlatform(Platform start) => startPlatform = start;
        public void SetEndPlatform(Platform end) => endPlatform = end;

        public void Update()
        {
            this.StateMachine?.Update();
        }
        
        private void OnChangedState(StateCategory category, Machine.IState state)
        {
            currentStateCategory = category;
            currentStateName = state.GetType().Name;
        }
        
        private void OnDestroy()
        {
            this.StateMachine.OnChangedState -= OnChangedState;
        }
    }
}