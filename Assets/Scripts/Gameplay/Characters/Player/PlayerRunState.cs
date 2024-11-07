using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerRunState : CharacterRunState
    {
        private GameObject currentTarget;

        private PlayerAttackState playerAttackState;
        private PlayerMoveState playerMoveState;

        public override void Initialize()
        {
            base.Initialize();
            playerAttackState = this.StateMachine.GetState<PlayerAttackState>();
            playerMoveState = this.StateMachine.GetState<PlayerMoveState>();
        }

        public override void Update()
        {
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }
            
            Move();
        }

        public void SetTarget(GameObject target)
        {
            currentTarget = target;
        }

        public override void Move()
        {
            if (GameObject.transform.position != currentTarget.transform.position)
            {
                var enemyGameObject = Calculate.Attack.CheckForwardEnemy(this.GameObject);
                if (!enemyGameObject)
                {
                    if (!playerMoveState.IsFacingTargetUsingDot(GameObject.transform, currentTarget.transform))
                    {
                        Calculate.Move.Rotate(GameObject.transform, currentTarget.transform, playerMoveState.RotationSpeed);
                        return;
                    }

                    GameObject.transform.position = Vector3.MoveTowards(GameObject.transform.position,
                        currentTarget.transform.position, MovementSpeed * Time.deltaTime);
                }
                else
                {
                    this.StateMachine.ExitCategory(Category);
                    this.StateMachine.SetStates(typeof(PlayerAttackState));
                }
            }
            else
            {
                this.StateMachine.ExitCategory(Category);
            }
        }
        
    }

    public class PlayerRunStateBuilder : CharacterRunStateBuilder
    {
        public PlayerRunStateBuilder() : base(new PlayerRunState())
        {
        }
    }
}