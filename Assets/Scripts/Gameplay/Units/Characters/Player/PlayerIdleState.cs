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
        
        private Vector3 targetPosition;

        private float checkEnemyCooldown = .01f;
        private float countCheckEnemyCooldown;
        private int asdf;

        private bool isCheckAttack;
        private bool isCheckJump;
        
        public GameObject TargetForMove { get; set; }
        public SO_PlayerMove SO_PlayerMove { get; set; }

        
        private bool IsNear(Vector3 current, Vector3 target, float threshold = 0.01f)
        {
            return (current - target).sqrMagnitude <= threshold;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            pathFinding = new PathFindingBuilder()
                .SetStartPosition(Vector3.zero)
                .SetEndPosition(Vector3.zero)
                .Build();
            playerSwitchAttackState = this.StateMachine.GetState<PlayerSwitchAttackState>();
        }

        public override void Enter()
        {
            base.Enter();
            
            isCheckJump = !this.StateMachine.IsActivateType(StateCategory.action, typeof(PlayerJumpState));
            isCheckAttack = isCheckJump;
            
            this.StateMachine.OnExitCategory += OnExitCategory;
        }

        public override void Update()
        {
            base.Update();
            CheckAttack();
            CheckMove();
            CheckJump();
        }

        public override void Exit()
        {
            base.Exit();
            TargetForMove = null;
            this.StateMachine.OnExitCategory -= OnExitCategory;
        }

        
        public void SetTarget(GameObject target)
        {
            //if(!target.TryGetComponent(out Platform platform)) return;
            
            this.TargetForMove = target;
            this.StateMachine.GetState<PlayerSwitchMoveState>().SetTarget(target);
        }
        
        private void CheckMove()
        {
            if(!TargetForMove) return;
            
            targetPosition = new Vector3(TargetForMove.transform.position.x, GameObject.transform.position.y, TargetForMove.transform.position.z);
            
            if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, targetPosition))
            {
                TargetForMove = null;
                ASD?.Invoke();
                OnFinishedToTarget?.Invoke();
            }
            else
            {
                this.StateMachine.GetState<PlayerSwitchMoveState>().SetTarget(TargetForMove);
                this.StateMachine.SetStates(typeof(PlayerSwitchMoveState));
                this.StateMachine.ExitCategory(Category);
            }
        }
        
        private void CheckAttack()
        {
            if(!isCheckAttack) return;
            
            countCheckEnemyCooldown += Time.deltaTime;
            if (countCheckEnemyCooldown > checkEnemyCooldown)
            {
                if (playerSwitchAttackState.IsFindUnitInRange())
                {
                    this.StateMachine.ExitOtherStates(typeof(PlayerSwitchAttackState));
                }

                countCheckEnemyCooldown = 0;
            }
        }
        
        private void OnExitCategory(Machine.IState state)
        {
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
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
                playerIdleState.TargetForMove = finish;

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