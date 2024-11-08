using Machine;
using ScriptableObjects.Character.Enemy;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character.Enemy
{
    public abstract class EnemyController : CharacterMainController
    {
        [SerializeField] protected SO_EnemyMove so_EnemyMove;
        
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;
        
        protected Platform startPlatform;
        protected Platform endPlatform;
        
        public StateMachine StateMachine { get; protected set; }
        
        
        public override void Initialize()
        {
            base.Initialize();

            CreateStates();
            
            this.StateMachine.Initialize();
            components.GetComponentInGameObjects<EnemyHealth>()?.Initialize();
            
            this.StateMachine.SetStates(typeof(EnemyIdleState));
            
            StateMachine.OnChangedState += OnChangedState;
        }

        protected virtual void CreateStates()
        {
            StateMachine = new StateMachine();
            
            var animation = components.GetComponentInGameObjects<EnemyAnimation>();

            var idleState = (EnemyIdleState)new EnemyIdleStateBuilder(new EnemyIdleState())
                .SetCharacterAnimation(animation)
                .SetIdleClip(so_EnemyMove.IdleClip)
                .SetStateMachine(this.StateMachine)
                .Build();;
            
            var attackState = (EnemyAttackState)new EnemyAttackStateBuilder(new EnemyAttackState())
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();

            var moveState = (EnemyMoveState)new EnemyMoveStateBuilder(new EnemyMoveState())
                .SetConfig(so_EnemyMove)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();

            var patrolState = (EnemyPatrolState)new EnemyPatrolStateBuilder(new EnemyPatrolState())
                .SetMovementSpeed(so_EnemyMove.WalkSpeed)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();
            
            this.StateMachine.AddStates(idleState, moveState, attackState);
        }
        
                
        public void SetStartPlatform(Platform start) => startPlatform = start;
        public void SetEndPlatform(Platform end) => endPlatform = end;

        public void Update()
        {
            StateMachine?.Update();
        }
        
        private void OnChangedState(StateCategory category, IState state)
        {
            currentStateCategory = category;
            currentStateName = state.GetType().Name;
        }
        
        private void OnDestroy()
        {
            StateMachine.OnChangedState -= OnChangedState;
        }
    }
}