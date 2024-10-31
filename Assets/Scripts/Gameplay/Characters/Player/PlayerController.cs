using System;
using System.Collections.Generic;
using Calculate;
using UnityEngine;

namespace Character
{
    public class PlayerController : CharacterMainController
    {
        public static event Action OnFinished;

        [SerializeField] private SO_PlayerMove so_PlayerMove;

        private StateMachine stateMachine;
        private PathToPoint pathToPoint;
        
        public override void Initialize()
        {
            base.Initialize();

            pathToPoint = new PathToPointBuilder()
                .SetPosition(transform, default)
                .Build();

            CreateStates();
            //Debug.Log(stateMachine.CheckState<PlayerIdleState>());
            stateMachine.GetState<PlayerIdleState>().OnFinishedMoveToEndTarget += OnFinishedMoveToEndTarget;
            stateMachine.SetStates(typeof(PlayerIdleState));
        }

        private void CreateStates()
        {
            stateMachine = new StateMachine();

            var idleState = (PlayerIdleState)new PlayerIdleStateBuilder()
                .SetPathToPoint(pathToPoint)
                .SetGameObject(gameObject)
                .SetIdleClip(so_PlayerMove.IdleClip)
                .SetCharacterAnimation(components.GetComponentInGameObjects<CharacterAnimation>())
                .SetStateMachine(stateMachine)
                .Build();
            
            var runState = (PlayerRunState)new PlayerRunStateBuilder()
                .SetCharacterAnimation(components.GetComponentInGameObjects<CharacterAnimation>())
                .SetGameObject(gameObject)
                .SetMovementSpeed(so_PlayerMove.RunSpeed)
                .SetRotateSpeed(so_PlayerMove.RotateSpeed)
                .SetStateMachine(stateMachine)
                .Build();

            var moveState = (PlayerMoveState)new PlayerMoveStateBuilder()
                .SetRunState(runState)
                .SetGameObject(gameObject)
                .SetStates(new IState[]{runState})
                .SetStateMachine(stateMachine)
                .Build();
            
            stateMachine.AddState(idleState);
            stateMachine.AddState(moveState);
        }

        private void Update()
        {
            stateMachine?.Update();
        }

        private void OnFinishedMoveToEndTarget()
        {
            OnFinished?.Invoke();
            //TODO: SetNextTarget
        }

        public void SetTarget(GameObject target)
        {
            stateMachine.GetState<PlayerIdleState>().SetFinishTarget(target);
        }

        private void OnDestroy()
        {
            stateMachine.GetState<PlayerIdleState>().OnFinishedMoveToEndTarget -= OnFinishedMoveToEndTarget;
        }
    }
}
