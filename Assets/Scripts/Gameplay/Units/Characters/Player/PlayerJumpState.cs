using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerJumpState : CharacterJumpState
    {
        private Gravity gravity;
        private CharacterController characterController;
        private IEndurance endurance;
        private Vector3 velocity;

        private KeyCode jumpKey;
        private int currentJumpCount;
        private int maxJumpCount;

        private readonly float cooldownCheckGround = .01f;
        private float jumpReductionEndurance;
        private float countCooldownCheckGround;
        private float currentGravity;

        public void SetCharacterController(CharacterController characterController) => this.characterController = characterController;
        public void SetEndurance(IEndurance endurance) => this.endurance = endurance;
        public void SetJumpKey(KeyCode jumpKey) => this.jumpKey = jumpKey;
        public void SetJumpReductionEndurance(float jumpReductionEndurance) => this.jumpReductionEndurance = jumpReductionEndurance;
        public void SetMaxJumpCount(int maxJumpCount) => this.maxJumpCount = maxJumpCount;

        
        public override void Initialize()
        {
            base.Initialize();
            gravity = gameObject.GetComponent<Gravity>();
            currentGravity = gravity.CurrentGravity;
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
                if (characterController.isGrounded)
                {
                    isCanExit = true;
                    characterAnimation.SetBlock(false);
                    this.StateMachine.ExitCategory(Category, null);
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
            velocity.y = 0f;
        }
        
        private void StartJump()
        {
            currentJumpCount++;
            characterAnimation.ChangeAnimationWithDuration(jumpClip, isForce: true);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * currentGravity);
            gravity.SetVelocityY(velocity.y);
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

        public PlayerJumpStateBuilder SetCharacterController(CharacterController characterController)
        {
            if(state is PlayerJumpState playerJumpState)
                playerJumpState.SetCharacterController(characterController);
            
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