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
        
        private Platform startPlatform;
        private Platform endPlatform;
        
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
            
            var characterAnimation = components.GetComponentInGameObjects<CharacterAnimation>();

            var idleState = (EnemyIdleState)new EnemyIdleStateBuilder(new EnemyIdleState())
                .SetCharacterAnimation(characterAnimation)
                .SetIdleClip(so_EnemyMove.IdleClip)
                .SetStateMachine(this.StateMachine)
                .Build();;
            
            var attackState = (EnemyAttackState)new EnemyAttackStateBuilder(new EnemyAttackState())
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();

            var moveState = (EnemyMoveState)new EnemyMoveStateBuilder(new EnemyMoveState())
                .SetRotationSpeed(so_EnemyMove.RotateSpeed)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();

            var patrolState = (EnemyPatrolState)new EnemyPatrolStateBuilder(new EnemyPatrolState())
                .SetStartPoint(startPlatform)
                .SetEndPoint(endPlatform)
                .SetMovementSpeed(so_EnemyMove.WalkSpeed)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();
            
            this.StateMachine.AddState(patrolState);
            this.StateMachine.AddState(idleState);
            this.StateMachine.AddState(moveState);
            this.StateMachine.AddState(attackState);
        }

        public void Update()
        {
            StateMachine?.Update();
        }
        
        
        public void SetStartPlatform(Platform start) => startPlatform = start;
        public void SetEndPlatform(Platform end) => endPlatform = end;
        
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