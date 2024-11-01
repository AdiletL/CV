using System;
using System.Collections.Generic;
using Calculate;
using Gameplay.Damage;
using ScriptableObjects.Character.Player;
using UnityEngine;

namespace Character.Player
{
    public class PlayerController : CharacterMainController
    {
        public static event Action OnFinished;

        [SerializeField] private SO_PlayerMove so_PlayerMove;
        [SerializeField] private SO_PlayerAttack so_PlayerAttack;

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

            var characterAnimation = components.GetComponentInGameObjects<CharacterAnimation>();

            var idleState = (PlayerIdleState)new PlayerIdleStateBuilder()
                .SetPathToPoint(pathToPoint)
                .SetGameObject(gameObject)
                .SetIdleClip(so_PlayerMove.IdleClip)
                .SetCharacterAnimation(characterAnimation)
                .SetStateMachine(stateMachine)
                .Build();
            
            var runState = (PlayerRunState)new PlayerBaseRunStateBuilder()
                .SetCharacterAnimation(characterAnimation)
                .SetGameObject(gameObject)
                .SetMovementSpeed(so_PlayerMove.RunSpeed)
                .SetStateMachine(stateMachine)
                .Build();

            var moveState = (PlayerMoveState)new PlayerMoveStateBuilder()
                .SetRunState(runState)
                .SetRotationSpeed(so_PlayerMove.RotateSpeed)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();

            var damageble = new NormalDamage(so_PlayerAttack.Damage);
            var meleeState = (PlayerMeleeAttackState)new PlayerMeleeAttackStateBuilder()
                .SetCharacterAnimation(characterAnimation)
                .SetAnimationClip(so_PlayerAttack.MeleeAttackClip)
                .SetDamageble(damageble)
                .SetStateMachine(stateMachine)
                .Build();

            var attackState = (PlayerAttackState)new PlayerAttackStateBuilder()
                .SetMeleeAttackState(meleeState)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
                
            
            stateMachine.AddState(idleState);
            stateMachine.AddState(moveState);
            stateMachine.AddState(attackState);
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
