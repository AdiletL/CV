using System.Collections.Generic;
using Machine;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterJumpState : State
    {
        public override StateCategory Category { get; } = StateCategory.jump;

        protected Gravity gravity;
        protected float currentGravity;
        
        private Vector3 velocity;

        public GameObject GameObject { get; set; }
        public AnimationClip JumpClip { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public CharacterController CharacterController { get; set; }
        public float JumpHeight { get; set; } = 1f;
        public int MaxJumpCount { get; set; }

        private int currentJumpCount;
        
        public override void Initialize()
        {
            gravity = GameObject.GetComponent<Gravity>();
            currentGravity = gravity.CurrentGravity;
        }

        public override void Enter()
        {
            base.Enter();
            CharacterAnimation.SetBlock(true);
            ClearValues();
            StartJump();
        }
        
        public override void Update()
        {
            CheckJump();
        }
        
        public override void LateUpdate()
        {
            if (CharacterController.isGrounded)
            {
                velocity.y = 0f;
                CharacterAnimation.SetBlock(false);
                this.StateMachine.ExitCategory(Category, null);
            }
        }

        public override void Exit()
        {
            base.Exit();
            if(CharacterController.isGrounded) return;
            velocity.y = 0f;
            CharacterAnimation.SetBlock(false);
        }

        private void ClearValues()
        {
            currentJumpCount = 0;
        }
        private void CheckJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount < MaxJumpCount)
            {
                StartJump();
            }
        }
        
        protected virtual void StartJump()
        {
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * currentGravity);
            currentJumpCount++;
            CharacterAnimation.ChangeAnimationWithDuration(JumpClip, isForce: true);
            gravity.SetVelocityY(velocity.y);
        }

    }
    
    public class CharacterJumpStateBuilder : StateBuilder<CharacterJumpState>
    {
        public CharacterJumpStateBuilder(CharacterJumpState instance) : base(instance)
        {
        }
        
        public CharacterJumpStateBuilder SetGameObject(GameObject gameObject)
        {
            state.GameObject = gameObject;
            return this;
        }
        
        public CharacterJumpStateBuilder SetJumpClip(AnimationClip clip)
        {
            state.JumpClip = clip;
            return this;
        }
        
        public CharacterJumpStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            state.CharacterAnimation = characterAnimation;
            return this;
        }
        
        public CharacterJumpStateBuilder SetMaxJumpCount(int amount)
        {
            state.MaxJumpCount = amount;
            return this;
        }
        public CharacterJumpStateBuilder SetJumpHeight(float value)
        {
            state.JumpHeight = value;
            return this;
        }
        public CharacterJumpStateBuilder SetCharacterController(CharacterController characterController)
        {
            state.CharacterController = characterController;
            return this;
        }
    }
}