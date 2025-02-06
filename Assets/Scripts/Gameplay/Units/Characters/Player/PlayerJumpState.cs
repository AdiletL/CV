using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerJumpState : CharacterJumpState
    {
        private PlayerKinematicControl playerKinematicControl;
        private IEndurance endurance;

        private KeyCode jumpKey;
        private int currentJumpCount;
        private int maxJumpCount;

        private readonly float cooldownCheckGround = .1f;
        private float jumpReductionEndurance;
        private float countCooldownCheckGround;
        private float currentGravity = -9;

        
        public void SetPlayerKinematicControl(PlayerKinematicControl control) => playerKinematicControl = control;
        public void SetEndurance(IEndurance endurance) => this.endurance = endurance;
        public void SetJumpKey(KeyCode jumpKey) => this.jumpKey = jumpKey;
        public void SetJumpReductionEndurance(float jumpReductionEndurance) => this.jumpReductionEndurance = jumpReductionEndurance;
        public void SetMaxJumpCount(int maxJumpCount) => this.maxJumpCount = maxJumpCount;

        
        public override void Initialize()
        {
            base.Initialize();
            SetPlayerKinematicControl(gameObject.GetComponent<PlayerKinematicControl>());
            characterAnimation.AddClip(jumpClip);
        }

        public override void Enter()
        {
            base.Enter();
            isCanExit = false;
            characterAnimation.SetBlock(true);
            ClearValues();
            StartJump();
        }

        public override void Update()
        {
            base.Update();
            if (countCooldownCheckGround > cooldownCheckGround)
            {
                if (Input.GetKeyDown(jumpKey) && currentJumpCount < maxJumpCount)
                {
                    StartJump();
                }
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            countCooldownCheckGround += Time.deltaTime;
            if (countCooldownCheckGround > cooldownCheckGround)
            {
                if (playerKinematicControl.IsGrounded)
                {
                    isCanExit = true;
                    characterAnimation.SetBlock(false);
                    this.stateMachine.ExitCategory(Category, null);
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
            isCanExit = true;
            characterAnimation.SetBlock(false);
        }

        private void ClearValues()
        {
            currentJumpCount = 0;
            countCooldownCheckGround = 0;
        }
        
        private void StartJump()
        {
            currentJumpCount++;
            var currentPower = jumpPower;
            if (currentJumpCount >= maxJumpCount)
                currentPower /= 1.5f;
            
            characterAnimation.ChangeAnimationWithDuration(jumpClip, isForce: true);
            playerKinematicControl.Jump(currentPower);
            playerKinematicControl.ForceUnground();
            ReductionEndurance();
        }
        
        private void ReductionEndurance()
        {
            endurance.RemoveEndurance(jumpReductionEndurance);
        }
    }
    
    public class PlayerJumpStateBuilder : CharacterJumpStateBuilder
    {
        public PlayerJumpStateBuilder() : base(new PlayerJumpState())
        {
        }

        public PlayerJumpStateBuilder SetEndurance(IEndurance endurance)
        {
            if(state is PlayerJumpState playerJumpState)
                playerJumpState.SetEndurance(endurance);
            
            return this;
        }
        public PlayerJumpStateBuilder SetJumpKey(KeyCode jumpKey)
        {
            if(state is PlayerJumpState playerJumpState)
                playerJumpState.SetJumpKey(jumpKey);
            
            return this;
        }
        public PlayerJumpStateBuilder SetReductionEndurance(float jumpReductionEndurance)
        {
            if(state is PlayerJumpState playerJumpState)
                playerJumpState.SetJumpReductionEndurance(jumpReductionEndurance);
            
            return this;
        }
        public PlayerJumpStateBuilder SetMaxJumpCount(int maxJumpCount)
        {
            if(state is PlayerJumpState playerJumpState)
                playerJumpState.SetMaxJumpCount(maxJumpCount);
            
            return this;
        }
    }
}