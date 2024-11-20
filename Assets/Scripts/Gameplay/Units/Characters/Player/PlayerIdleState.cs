using System;
using System.Collections.Generic;
using Calculate;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;


namespace Unit.Character.Player
{
    public class PlayerIdleState : CharacterIdleState
    {
        public event Action OnFinishedToTarget;
        public static event Action ASD;
        
        private PathFinding pathFinding;
        private PlayerSwitchAttackState playerSwitchAttackState;

        private float checkEnemyCooldown = .25f;
        private float countCheckEnemyCooldown;
        private int asdf;

        private bool isCheckAttack;
        private bool isCheckJump;
        
        private Queue<Platform> pathToPoint = new();
        
        
        public GameObject FinishTargetForMove { get; set; }
        public SO_PlayerMove SO_PlayerMove { get; set; }

        
        public override void Initialize()
        {
            base.Initialize();
            pathFinding = new PathToPointBuilder()
                .SetPosition(this.GameObject.transform, FinishTargetForMove.transform)
                .Build();
            playerSwitchAttackState = this.StateMachine.GetState<PlayerSwitchAttackState>();
        }

        public override void Enter()
        {
            base.Enter();
            pathToPoint.Clear();
            
            isCheckJump = !this.StateMachine.IsActivateType(StateCategory.action, typeof(PlayerJumpState));
            isCheckAttack = isCheckJump;
            
            this.StateMachine.OnExitCategory += OnExitCategory;
        }

        public override void LateUpdate()
        {
            CheckAttack();
            CheckMove();
            CheckJump();
        }

        public override void Exit()
        {
            base.Exit();
            this.StateMachine.OnExitCategory -= OnExitCategory;
        }

        public void SetFinishTarget(GameObject finish)
        {
            this.FinishTargetForMove = finish;
            pathFinding.SetTarget(finish.transform);
        }

        private void CheckMove()
        {
            if (GameObject.transform.position.x == FinishTargetForMove.transform.position.x 
                && GameObject.transform.position.z == FinishTargetForMove.transform.position.z)
            {
                pathToPoint.Clear();
                ASD?.Invoke();
                OnFinishedToTarget?.Invoke();
            }
            else
            {
                if (FinishTargetForMove && pathToPoint.Count == 0) FindPathToPoint();
                if (pathToPoint.Count == 0) return;

                this.StateMachine.ExitCategory(Category);
                this.StateMachine.GetState<PlayerSwitchMoveState>().SetPathToFinish(pathToPoint);
                this.StateMachine.GetState<PlayerSwitchMoveState>().SetFinish(FinishTargetForMove);
                this.StateMachine.SetStates(typeof(PlayerSwitchMoveState));
            }
        }
        private void FindPathToPoint()
        {
            if (pathToPoint.Count == 0)
            {
                pathToPoint = pathFinding.GetPath();
            }
        }
        
        private void CheckAttack()
        {
            if(!isCheckAttack) return;
            
            countCheckEnemyCooldown += Time.deltaTime;
            if (countCheckEnemyCooldown > checkEnemyCooldown)
            {
                if (playerSwitchAttackState.IsCheckTarget())
                {
                    this.StateMachine.ExitOtherCategories(Category);
                    this.StateMachine.SetStates(typeof(PlayerSwitchAttackState));
                }

                countCheckEnemyCooldown = 0;
            }
        }
        
        private void OnExitCategory(Machine.StateCategory category, Machine.IState state)
        {
            if (category == Machine.StateCategory.action
                && state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
            {
                PlayAnimation();
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
            isCheckJump = false;
            isCheckAttack = false;
        }
    }
    

    public class PlayerIdleStateBuilder : CharacterIdleStateBuilder
    {
        public PlayerIdleStateBuilder() : base(new PlayerIdleState())
        {
            
        }

        public PlayerIdleStateBuilder SetFinishTargetToMove(GameObject finish)
        {
            if (state is PlayerIdleState playerIdleState)
                playerIdleState.FinishTargetForMove = finish;

            return this;
        }
        
        public PlayerIdleStateBuilder SetMoveConfig(SO_PlayerMove config)
        {
            if (state is PlayerIdleState playerIdleState)
                playerIdleState.SO_PlayerMove = config;

            return this;
        }
    }
}