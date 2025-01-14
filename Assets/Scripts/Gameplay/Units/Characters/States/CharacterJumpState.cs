using System.Collections.Generic;
using Machine;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterJumpState : State
    {
        public override StateCategory Category { get; } = StateCategory.jump;

        protected Gravity gravity;
        
        private Vector3 startPosition;
        private int countJump;
        private float timer;
        private float progress;
        private bool isJumping;
        
        public GameObject GameObject { get; set; }
        public AnimationCurve Curve { get; set; }
        public AnimationClip JumpClip { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public float JumpDuration { get; set; }
        public float JumpHeight { get; set; }
        public int MaxJumpCount { get; set; }
        
        public override void Initialize()
        {
            gravity = GameObject.GetComponent<Gravity>();
        }

        public override void Enter()
        {
            startPosition = GameObject.transform.position;
            isJumping = true;
            CharacterAnimation.ChangeAnimationWithDuration(JumpClip, duration: JumpDuration, isForce: true);
            CharacterAnimation.SetBlock(true);
            gravity.InActivateGravity();
            countJump = 1;
            progress = 0;
        }
        
        public override void Update()
        {
            CheckJump();
        }
        
        public override void LateUpdate()
        {
            if (isJumping)
            {
                timer += Time.deltaTime;
                progress = (timer / JumpDuration);

                if (progress >= .5f)
                {
                    gravity.ActivateGravity();
                    if (Physics.Raycast(GameObject.transform.position, Vector3.down,.2f))
                    {
                        isJumping = false;
                        timer = 0f;
                        CharacterAnimation.SetBlock(false);
                        this.StateMachine.ExitCategory(Category, null);
                    }
                }
                else
                {
                    float jumpOffset = Curve.Evaluate(progress) * JumpHeight;
                    GameObject.transform.position = new Vector3(GameObject.transform.position.x, startPosition.y + jumpOffset, GameObject.transform.position.z);
                }
            }
        }

        private void CheckJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && countJump < MaxJumpCount)
            {
                StartJump();
            }
        }
        
        protected virtual void StartJump()
        {
            startPosition = GameObject.transform.position;
            timer = 0f;
            CharacterAnimation.ChangeAnimationWithDuration(JumpClip, duration: JumpDuration, isForce: true);
            gravity.InActivateGravity();
            countJump++;
            progress = 0;
        }
        public override void Exit()
        {
            
        }
    }
    
    public class CharacterJumpStateBuilder : StateBuilder<CharacterJumpState>
    {
        public CharacterJumpStateBuilder(CharacterJumpState instance) : base(instance)
        {
        }

        public CharacterJumpStateBuilder SetJumpDuration(float duration)
        {
            state.JumpDuration = duration;
            return this;
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
        
        public CharacterJumpStateBuilder SetAnimationCurve(AnimationCurve curve)
        {
            state.Curve = curve;
            return this;
        }

        public CharacterJumpStateBuilder SetJumpHeight(float jumpHeight)
        {
            state.JumpHeight = jumpHeight;
            return this;
        }
        public CharacterJumpStateBuilder SetMaxJumpCount(int amount)
        {
            state.MaxJumpCount = amount;
            return this;
        }
    }
}