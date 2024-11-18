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

        private float cooldownCheckEnemy = .3f;
        private float countCooldownCheckEnemy;

        private Queue<Platform> pathToPoint = new();
        
        public Transform Center { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            playerSwitchAttackState = this.StateMachine.GetState<PlayerSwitchAttackState>();
            playerSwitchMoveState = this.StateMachine.GetState<PlayerSwitchMoveState>();
        }

        public override void Enter()
        {
            base.Enter();
            countCooldownCheckEnemy = cooldownCheckEnemy;
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
            CheckEnemy();
        }
        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
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
                if (!Calculate.Move.IsFacingTargetUsingAngle(GameObject.transform, currentTarget.transform))
                {
                    Calculate.Move.Rotate(GameObject.transform, currentTarget.transform, playerSwitchMoveState.RotationSpeed);
                    return;
                }

                GameObject.transform.position = Vector3.MoveTowards(GameObject.transform.position,
                    currentTarget.transform.position, MovementSpeed * Time.deltaTime);
            }
        }

        private void CheckEnemy()
        {
            countCooldownCheckEnemy += Time.deltaTime;
            if (countCooldownCheckEnemy > cooldownCheckEnemy)
            {
                if (playerSwitchAttackState.IsCheckTarget())
                {
                    this.StateMachine.ExitCategory(Category);
                    this.StateMachine.SetStates(typeof(PlayerSwitchAttackState));
                }
                countCooldownCheckEnemy = 0;
            }
        }
        
        private void FindNextPoint()
        {
            if (pathToPoint.Count == 0) return;
            currentTarget = pathToPoint.Peek().gameObject;
        }
        
    }

    public class PlayerRunStateBuilder : CharacterRunStateBuilder
    {
        public PlayerRunStateBuilder() : base(new PlayerRunState())
        {
        }
        
        
        public PlayerRunStateBuilder SetCenter(Transform center)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.Center = center;
            
            return this;
        }
        
    }
}