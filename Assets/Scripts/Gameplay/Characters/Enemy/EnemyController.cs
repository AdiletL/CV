using ScriptableObjects.Character.Enemy;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character.Enemy
{
    public abstract class EnemyController : CharacterMainController
    {
        [SerializeField] protected SO_EnemyMove so_EnemyMove;
        
        public StateMachine StateMachine { get; set; }
        
        public override void Initialize()
        {
            base.Initialize();

            CreateStates();
            
            components.GetComponentInGameObjects<EnemyHealth>()?.Initialize();
        }

        private void CreateStates()
        {
            StateMachine = new StateMachine();
            
            var characterAnimation = components.GetComponentInGameObjects<CharacterAnimation>();

            var idleState = (EnemyIdleState)new EnemyIdleStateBuilder(new EnemyIdleState())
                .SetCharacterAnimation(characterAnimation)
                .SetIdleClip(so_EnemyMove.IdleClip)
                .SetStateMachine(this.StateMachine)
                .Build();;
            
            this.StateMachine.AddState(idleState);
            this.StateMachine.Initialize();
            this.StateMachine.SetStates(typeof(EnemyIdleState));
        }

        public void Update()
        {
            StateMachine?.Update();
        }
    }
}