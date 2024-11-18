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
            StateMachine.SetStates(typeof(PlayerIdleState));
        }

        private void CreateStates()
        {
            StateMachine = new StateMachine();

            var characterAnimation = components.GetComponentFromArray<CharacterAnimation>();
            var center = components.GetComponentFromArray<UnitCenter>().Center;
            var enemyLayer = Layers.CREEP_LAYER;
            
            var idleState = (PlayerIdleState)new PlayerIdleStateBuilder()
                .SetFinishTargetToMove(finish)
                .SetIdleClips(so_PlayerMove.IdleClip)
                .SetCharacterAnimation(characterAnimation)
                .SetCenter(center)
                .SetGameObject(gameObject)
                .SetStateMachine(StateMachine)
                .Build();

            var moveState = (PlayerSwitchMoveState)new PlayerMoveStateBuilder()
                .SetCenter(center)
                .SetCharacterAnimation(characterAnimation)
                .SetConfig(so_PlayerMove)
                .SetGameObject(gameObject)
                .SetStateMachine(StateMachine)
                .Build();
            
            var attackState = (PlayerSwitchAttackState)new PlayerAttackStateBuilder()
                .SetCenter(center)
                .SetEnemyLayer(enemyLayer)
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
            this.StateMachine?.GetState<PlayerIdleState>()?.SetFinishTarget(finish);
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
