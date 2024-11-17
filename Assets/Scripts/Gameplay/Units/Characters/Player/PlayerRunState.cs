using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerRunState : CharacterRunState
    {
        private GameObject currentTarget;

        private PlayerSwitchAttackState playerSwitchAttackState;
        private PlayerSwitchMoveState playerSwitchMoveState;
        
        private GameObject finishTargetForMove;
        
        private Vector3 currentFinishPosition;

        private Queue<Platform> pathToPoint = new();

        public override void Initialize()
        {
            base.Initialize();
            playerSwitchAttackState = this.StateMachine.GetState<PlayerSwitchAttackState>();
            playerSwitchMoveState = this.StateMachine.GetState<PlayerSwitchMoveState>();
        }

        public override void Update()
        {
            if (!currentTarget || finishTargetForMove.transform.position == currentFinishPosition)
                FindNextPoint();
            
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }
            Move();
        }
        
        public void SetFinish(GameObject target)
        {
            finishTargetForMove = target;
            currentFinishPosition = target.transform.position;
        }
        public void SetPathToFinish(Queue<Platform> pathToPoint)
        {
            this.pathToPoint = pathToPoint;
        }

        public override void Move()
        {
            if (GameObject.transform.position == currentTarget.transform.position)
            {
                currentTarget = null;
                pathToPoint.Dequeue();
            }
            else
            {
                var enemyGameObject = Calculate.Attack.CheckForwardEnemy(this.GameObject, Layers.ENEMY_LAYER);
                if (!enemyGameObject)
                {
                    if (!Calculate.Move.IsFacingTargetUsingDot(GameObject.transform, currentTarget.transform))
                    {
                        Calculate.Move.Rotate(GameObject.transform, currentTarget.transform, playerSwitchMoveState.RotationSpeed);
                        return;
                    }

                    GameObject.transform.position = Vector3.MoveTowards(GameObject.transform.position,
                        currentTarget.transform.position, MovementSpeed * Time.deltaTime);
                }
                else
                {
                    this.StateMachine.ExitCategory(Category);
                    this.StateMachine.SetStates(typeof(PlayerSwitchAttackState));
                }
            }
        }
        
        private void FindNextPoint()
        {
            if(pathToPoint.Count == 0) return;
            
            currentTarget = pathToPoint.Peek().gameObject;
        }

        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
        }
    }

    public class PlayerRunStateBuilder : CharacterRunStateBuilder
    {
        public PlayerRunStateBuilder() : base(new PlayerRunState())
        {
        }
    }
}