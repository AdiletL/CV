using System;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using Unity.Collections;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerController : CharacterMainController
    {
        public override UnitType UnitType { get; } = UnitType.player;
        
        [SerializeField] private SO_PlayerMove so_PlayerMove;
        [SerializeField] private SO_PlayerAttack so_PlayerAttack;
        
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;

        private GameObject finish;


        public override void Initialize()
        {
            base.Initialize();

            CreateStates();
            
            StateMachine.Initialize();
            
            components.GetComponentFromArray<PlayerHealth>()?.Initialize();
            
            //Debug.Log(stateMachine.CheckState<PlayerIdleState>());
            StateMachine.OnChangedState += OnChangedState;
            StateMachine.SetStates(typeof(PlayerIdleIdleState));
        }

        private void CreateStates()
        {
            StateMachine = new StateMachine();

            var characterAnimation = components.GetComponentFromArray<CharacterAnimation>();

            var idleState = (PlayerIdleIdleState)new PlayerIdleStateBuilder()
                .SetFinishTargetToMove(finish)
                .SetIdleClips(so_PlayerMove.IdleClip)
                .SetCharacterAnimation(characterAnimation)
                .SetGameObject(gameObject)
                .SetStateMachine(StateMachine)
                .Build();

            var moveState = (PlayerSwitchMoveState)new PlayerMoveStateBuilder()
                .SetCharacterAnimation(characterAnimation)
                .SetConfig(so_PlayerMove)
                .SetGameObject(gameObject)
                .SetStateMachine(StateMachine)
                .Build();
            
            var attackState = (PlayerSwitchAttackState)new PlayerAttackStateBuilder()
                .SetGameObject(gameObject)
                .SetCharacterAnimation(characterAnimation)
                .SetConfig(so_PlayerAttack)
                .SetStateMachine(StateMachine)
                .Build();
                
            
            StateMachine.AddStates(idleState, attackState, moveState);
        }

        private void Update()
        {
            StateMachine?.Update();
        }

        public void SetFinishTarget(GameObject target)
        { 
            finish = target;
            this.StateMachine?.GetState<PlayerIdleIdleState>()?.SetFinishTarget(finish);
        }

        private void OnChangedState(StateCategory category, Machine.IState state)
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
