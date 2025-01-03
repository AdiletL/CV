﻿using System;
using System.Collections.Generic;
using Calculate;
using Machine;
using Movement;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerRunState : CharacterRunState
    {
        private PlayerSwitchAttackState playerSwitchAttackState;
        private Rotation rotation;
        private PathFinding pathFinding;

        private GameObject finalTarget;
        private Transform currentTarget;
        private Vector3 finalTargetPosition;
        private Vector3 currentTargetPosition;
        private Vector3 direction;

        private readonly float cooldownCheckEnemy = 0.1f;
        private float countCooldownCheckEnemy;

        private bool isRotate;
        private bool isCheckEnemy;
        private bool isCheckJump;

        private Queue<Platform> pathToPoint = new();

        public SO_PlayerMove SO_PlayerMove { get; set; }
        public Transform Center { get; set; }
        public CharacterController CharacterController { get; set; }
        public float RotationSpeed { get; set; }
        

        public override void Initialize()
        {
            base.Initialize();
            playerSwitchAttackState = this.StateMachine.GetState<PlayerSwitchAttackState>();
            rotation = new Rotation(GameObject.transform, RotationSpeed);
            pathFinding = new PathFindingBuilder()
                .SetStartPosition(GameObject.transform.position)
                .SetEndPosition(GameObject.transform.position)
                .Build();
        }

        public override void Enter()
        {
            base.Enter();
            countCooldownCheckEnemy = cooldownCheckEnemy;
            
            isCheckJump = !this.StateMachine.IsActivateType(StateCategory.action, typeof(PlayerJumpState));
            isCheckEnemy = isCheckJump;
            
            this.StateMachine.OnExitCategory += OnExitCategory;
        }

        public override void Update()
        {
            base.Update();
            
            if (!currentTarget)
                FindNextPoint();
            
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }

            CheckFinalTargetPosition();
            Move();
        }   

        public override void LateUpdate()
        {
            CheckJump();
            //CheckEnemy();
        }

        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
            this.StateMachine.OnExitCategory -= OnExitCategory;
        }
        
        public void SetTarget(GameObject target)
        {
            currentTarget = null;
            finalTarget = target;
            finalTargetPosition = target.transform.position;
            pathFinding.SetStartPosition(GameObject.transform.position);
            pathFinding.SetTargetPosition(target.transform.position);
            ResetColorOnPath();
        }
        
        private void ResetColorOnPath()
        {
            for (int i = pathToPoint.Count - 1; i >= 0; i--)
                pathToPoint.Dequeue()?.GetComponent<UnitRenderer>()?.ResetColor();
        }

        private void FindPathToPoint()
        {
            pathToPoint = pathFinding.GetPath(true);
        }
        private void FindNextPoint()
        {
            if(pathToPoint.Count == 0)
                FindPathToPoint();
            
            if(pathToPoint.Count == 0) return;
            
            currentTarget = pathToPoint.Peek().transform;
            rotation.SetTarget(currentTarget);
        }
        
        public override void Move()
        {
            currentTargetPosition = new Vector3(currentTarget.position.x, GameObject.transform.position.y, currentTarget.position.z);
            
            if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, currentTargetPosition))
            {
                pathToPoint.Peek()?.GetComponent<UnitRenderer>().ResetColor();
                currentTarget = null; 
                pathToPoint.Dequeue();

                CheckFinishedToFinalTarget();
            }
            else
            {
                if (!Calculate.Move.IsFacingTargetUsingAngle(GameObject.transform.position, GameObject.transform.forward, currentTargetPosition))
                {
                    rotation.Rotate();
                    return;
                }
                direction = (currentTargetPosition - GameObject.transform.position).normalized;
                CharacterController.Move(direction * (MovementSpeed * Time.deltaTime));
            }
        }

        private void CheckFinalTargetPosition()
        {
            if (!Calculate.Distance.IsNearUsingSqr(finalTarget.transform.position, finalTargetPosition))
            {
                pathToPoint.Clear();
                FindNextPoint();
            }
        }
        private void CheckFinishedToFinalTarget()
        {
            if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, finalTarget.transform.position)
                || pathToPoint.Count == 0)
                this.StateMachine.ExitCategory(Category);
        }
        
        private void CheckEnemy()
        {
            if(!isCheckEnemy) return;
            
            countCooldownCheckEnemy += Time.deltaTime;
            if (countCooldownCheckEnemy > cooldownCheckEnemy)
            {
                if (playerSwitchAttackState.IsFindUnitInRange())
                {
                    foreach (var VARIABLE in pathToPoint)
                        VARIABLE.SetColor(Color.white);
                    
                    this.StateMachine.SetStates(typeof(PlayerSwitchAttackState));
                    this.StateMachine.ExitCategory(Category);
                }
                countCooldownCheckEnemy = 0;
            }
        }

        private void OnExitCategory(Machine.IState state)
        {
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
            {
                PlayAnimation();
                isCheckEnemy = true;
                isCheckJump = true;
            }
        }

        private void CheckJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isCheckJump)
            {
                StartJump();
            }
        }
        private void StartJump()
        {
            if (!this.StateMachine.IsStateNotNull(typeof(PlayerJumpState)))
            {
                var playerJumpState = (PlayerJumpState)new PlayerJumpStateBuilder()
                    .SetMaxJumpCount(SO_PlayerMove.MaxJumpCount)
                    .SetAnimationCurve(SO_PlayerMove.JumpCurve)
                    .SetJumpDuration(SO_PlayerMove.JumpDuration)
                    .SetJumpClip(SO_PlayerMove.JumpClip)
                    .SetJumpHeight(SO_PlayerMove.JumpHeight)
                    .SetGameObject(GameObject)
                    .SetCharacterAnimation(CharacterAnimation)
                    .SetStateMachine(this.StateMachine)
                    .Build();
                        
                playerJumpState.Initialize();
                this.StateMachine.AddStates(playerJumpState);
            }
            this.StateMachine.SetStates(typeof(PlayerJumpState));
            isCheckEnemy = false;
            isCheckJump = false;
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

        public PlayerRunStateBuilder SetMoveConfig(SO_PlayerMove config)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.SO_PlayerMove = config;
            
            return this;  
        }
        public PlayerRunStateBuilder SetCharacterController(CharacterController characterController)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.CharacterController = characterController;
            
            return this;  
        }
        public PlayerRunStateBuilder SetRotationSpeed(float rotationSpeed)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.RotationSpeed = rotationSpeed;
            
            return this;  
        }
    }
}