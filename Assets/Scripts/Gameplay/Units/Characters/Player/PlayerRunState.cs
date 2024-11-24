using System;
using System.Collections.Generic;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerRunState : CharacterRunState
    {
        private GameObject currentTarget;

        private PlayerSwitchAttackState playerSwitchAttackState;
        private PlayerSwitchMoveState playerSwitchMoveState;
        
        private GameObject finish;
        
        private Vector3 currentFinishPosition;

        private readonly float cooldownCheckEnemy = 0.1f;
        private float countCooldownCheckEnemy;

        private bool isRotate;
        private bool isCheckEnemy;
        private bool isCheckJump;

        private Queue<Platform> pathToPoint = new();

        public Transform Center { get; set; }
        public SO_PlayerMove SO_PlayerMove { get; set; }
        
        private bool IsNear(Vector3 current, Vector3 target, float threshold = 0)
        {
            return (current - target).sqrMagnitude <= threshold;
        }

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
            
            isCheckJump = !this.StateMachine.IsActivateType(StateCategory.action, typeof(PlayerJumpState));
            isCheckEnemy = isCheckJump;
            
            this.StateMachine.OnExitCategory += OnExitCategory;
        }

        public override void Update()
        {
            base.Update();
            
            if (!currentTarget || finish.transform.position == currentFinishPosition)
                FindNextPoint();
            
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }
            
            Move();
        }

        public override void LateUpdate()
        {
            CheckJump();
            CheckEnemy();
        }

        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
            this.StateMachine.OnExitCategory -= OnExitCategory;
        }
        
        public void SetFinish(GameObject target)
        {
            finish = target;
            currentFinishPosition = target.transform.position;
        }
        public void SetPathToFinish(Queue<Platform> pathToPoint)
        {
            this.pathToPoint = pathToPoint;
        }
        
        public override void Move()
        {
            currentFinishPosition = new Vector3(currentTarget.transform.position.x, GameObject.transform.position.y, currentTarget.transform.position.z);
            
            if (IsNear(GameObject.transform.position, currentFinishPosition))
            {
                currentTarget = null; 
                pathToPoint.Dequeue();
            }
            else
            {
                if (!Calculate.Move.IsFacingTargetUsingAngle(GameObject.transform, currentTarget.transform))
                {
                    Calculate.Move.Rotate(GameObject.transform, currentTarget.transform, playerSwitchMoveState.RotationSpeed, ignoreX: true, ignoreZ: true, ignoreY: false);
                    return;
                }
    
                GameObject.transform.position = Vector3.MoveTowards(GameObject.transform.position, currentFinishPosition, MovementSpeed * Time.deltaTime);
            }
        }

        private void CheckEnemy()
        {
            if(!isCheckEnemy) return;
            
            countCooldownCheckEnemy += Time.deltaTime;
            if (countCooldownCheckEnemy > cooldownCheckEnemy)
            {
                if (playerSwitchAttackState.IsFindUnitInRange())
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

        private void OnExitCategory(Machine.StateCategory category, Machine.IState state)
        {
            if (category == Machine.StateCategory.action 
                && state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
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
    }
}