using System;
using Gameplay.Characters.Player;
using Machine;
using ScriptableObjects.Character.Player;
using Unity.Collections;
using UnityEngine;

namespace Character.Player
{
    public class PlayerController : CharacterMainController
    {
        [SerializeField] private SO_PlayerMove so_PlayerMove;
        [SerializeField] private SO_PlayerAttack so_PlayerAttack;
        
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;

        private GameObject finish;
        
        public override void Initialize()
        {
            base.Initialize();

            CreateStates();
            
            stateMachine.Initialize();
            
            components.GetComponentInGameObjects<PlayerHealth>()?.Initialize();
            
            //Debug.Log(stateMachine.CheckState<PlayerIdleState>());
            stateMachine.OnChangedState += OnChangedState;
            stateMachine.SetStates(typeof(PlayerIdleState));
        }

        private void CreateStates()
        {
            stateMachine = new StateMachine();

            var characterAnimation = components.GetComponentInGameObjects<CharacterAnimation>();

            var idleState = (PlayerIdleState)new PlayerIdleStateBuilder()
                .SetGameObject(gameObject)
                .SetEndPoint(finish.transform)
                .SetIdleClip(so_PlayerMove.IdleClip)
                .SetCharacterAnimation(characterAnimation)
                .SetStateMachine(stateMachine)
                .Build();

            var moveState = (PlayerMoveState)new PlayerMoveStateBuilder()
                .SetCharacterAnimation(characterAnimation)
                .SetConfig(so_PlayerMove)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
            
            var attackState = (PlayerAttackState)new PlayerAttackStateBuilder()
                .SetGameObject(gameObject)
                .SetCharacterAnimation(characterAnimation)
                .SetConfig(so_PlayerAttack)
                .SetStateMachine(stateMachine)
                .Build();
                
            
            stateMachine.AddStates(idleState, attackState, moveState);
        }

        private void Update()
        {
            stateMachine?.Update();
        }

        public void SetFinishTarget(GameObject target)
        { 
            finish = target;
            this.stateMachine?.GetState<PlayerIdleState>()?.SetFinishTarget(finish);
        }

        private void OnChangedState(StateCategory category, Machine.IState state)
        {
            currentStateCategory = category;
            currentStateName = state.GetType().Name;
        }

        private void OnDestroy()
        {
            stateMachine.OnChangedState -= OnChangedState;
        }
    }
}
