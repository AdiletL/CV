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
        
        protected Vector3 velocity;

        public GameObject GameObject { get; set; }
        public AnimationClip JumpClip { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public CharacterController CharacterController { get; set; }
        public float JumpHeight { get; set; } = 1f;
        public int MaxJumpCount { get; set; }

        protected int currentJumpCount;
        
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
        
        protected virtual void CheckJump()
        {
            
        }

        private void ClearValues()
        {
            currentJumpCount = 0;
        }
        
        protected virtual void StartJump()
        {
            currentJumpCount++;
            CharacterAnimation.ChangeAnimationWithDuration(JumpClip, isForce: true);
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * currentGravity);
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