using System;
using System.Collections;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;


namespace Unit.Character.Player
{
    public class PlayerIdleState : CharacterIdleState
    {
        public event Action OnFinishedToTarget;
        
        private PlayerSwitchAttackState playerSwitchAttackState;

        private Vector3 targetPosition;

        private float cooldownCheckAttack = 1, countCooldownCheckAttack;
        private float cooldownCheckEnemy = .01f;
        private float countCooldownCheckEnemy;

        private bool isCheckAttack;
        private bool isCheckJump;
        
        public GameObject TargetForMove { get; set; }
        public SO_PlayerMove SO_PlayerMove { get; set; }
        
        
        public override void Initialize()
        {
            base.Initialize();
            playerSwitchAttackState = this.StateMachine.GetState<PlayerSwitchAttackState>();
        }

        public override void Enter()
        {
            base.Enter();
            
            isCheckJump = !this.StateMachine.IsActivateType(typeof(PlayerJumpState));
            isCheckAttack = isCheckJump;
            countCooldownCheckAttack = 0;
            
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
            this.TargetForMove = target;
            this.StateMachine.GetState<PlayerSwitchMoveState>().SetTarget(target);
            countCooldownCheckAttack = cooldownCheckAttack;
        }
        
        private void CheckMove()
        {
            if(!TargetForMove) return;
            
            targetPosition = new Vector3(TargetForMove.transform.position.x, GameObject.transform.position.y, TargetForMove.transform.position.z);
            
            if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, targetPosition))
            {
                TargetForMove = null;
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

            if (countCooldownCheckAttack <= 0)
            {
                countCooldownCheckAttack = 0;
            }
            else
            {
                countCooldownCheckAttack -= Time.deltaTime;
                return;
            }
                
            
            countCooldownCheckEnemy += Time.deltaTime;
            if (countCooldownCheckEnemy > cooldownCheckEnemy)
            {
                if (playerSwitchAttackState.IsFindUnitInRange())
                {
                    this.StateMachine.ExitOtherStates(typeof(PlayerSwitchAttackState));
                }

                countCooldownCheckEnemy = 0;
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