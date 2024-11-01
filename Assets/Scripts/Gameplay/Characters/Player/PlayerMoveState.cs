using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerMoveState : CharacterMoveState
    {
        private GameObject currentTarget;
        private Quaternion currentTargetForRotate;

        public PlayerRunState runState;
        public float RotationSpeed;

        public bool IsLookAtTarget()
        {
            if (this.GameObject.transform.rotation == this.currentTargetForRotate)
                return true;
            else
                return false;
        }

        public void SetTarget(GameObject target)
        {
            currentTarget = target;
            MoveStateMachine.GetState<PlayerRunState>()?.SetTarget(target);
        }
        
        protected override void DestermineState()
        {
            if(!currentTarget) return;
            
            if (GameObject.transform.position != currentTarget.transform.position)
            {
                var enemyGameObject = this.StateMachine.GetState<PlayerAttackState>().CheckForwardEnemy();
                if (!enemyGameObject)
                {
                    Rotate(currentTarget);
                    if (GameObject.transform.rotation == currentTargetForRotate)
                        MoveStateMachine.SetStates(runState.GetType());
                }
                else
                {
                    this.StateMachine.SetStates(typeof(PlayerAttackState));
                    Debug.Log("PlayerAttack");
                }
            }
            else
            {
                this.StateMachine.SetStates(typeof(PlayerIdleState));
            }
        }
        
        public void Rotate(GameObject target)
        {
           var direction = target.transform.position - GameObject.transform.position;
            if (direction == Vector3.zero) return;
            
            currentTargetForRotate = Quaternion.LookRotation(direction, Vector3.up);
            GameObject.transform.rotation = Quaternion.RotateTowards(GameObject.transform.rotation, currentTargetForRotate, RotationSpeed * Time.deltaTime);
        }
    }

    public class PlayerMoveStateBuilder : CharacterMoveStateBuilder
    {
        public PlayerMoveStateBuilder() : base(new PlayerMoveState())
        {
        }

        public PlayerMoveStateBuilder SetRotationSpeed(float speed)
        {
            if (state is PlayerMoveState playerMoveState)
            {
                playerMoveState.RotationSpeed = speed;
            }

            return this;
        }
        public PlayerMoveStateBuilder SetRunState(IState runState)
        {
            if (runState is PlayerRunState playerRunState)
            {
                if (state is PlayerMoveState playerMoveState)
                {
                    playerMoveState.MoveStateMachine.AddState(playerRunState);
                    playerMoveState.runState = playerRunState;
                }
            }

            return this;
        }
    }
}