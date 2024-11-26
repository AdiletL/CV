using System.Collections.Generic;
using Machine;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterJumpState : State
    {
        public override StateCategory Category { get; } = StateCategory.action;

        protected Gravity gravity;
        
        private Vector3 startPosition;
        private float timer;
        private bool isJumping;
        
        public GameObject GameObject { get; set; }
        public AnimationCurve Curve { get; set; }
        public AnimationClip JumpClip { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public float JumpDuration { get; set; }
        public float JumpHeight { get; set; }
        
        public override void Initialize()
        {
            gravity = GameObject.GetComponent<Gravity>();
        }

        public override void Enter()
        {
            startPosition = GameObject.transform.position;
            isJumping = true;
            CharacterAnimation.ChangeAnimation(JumpClip, duration: JumpDuration);
            CharacterAnimation.SetBlock(true);
            gravity.ChangeGravity(false);
        }

        public override void Update()
        {
            
        }
        
        public override void LateUpdate()
        {
            CheckJump();
            if (isJumping)
            {
                timer += Time.deltaTime;
                float progress = (timer / JumpDuration);

                if (progress >= .5f)
                {
                    gravity.ChangeGravity(true);
                    if (progress >= 1f)
                    {
                        isJumping = false;
                        timer = 0f;
                        CharacterAnimation.SetBlock(false);
                        this.StateMachine.ExitCategory(Category);
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartJump();
            }
        }
        
        private void StartJump()
        {
            startPosition = GameObject.transform.position;
            timer = 0f;
            CharacterAnimation.ChangeAnimation(JumpClip, duration: JumpDuration, isForce: true);
            gravity.ChangeGravity(false);
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
    }
}