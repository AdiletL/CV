using System.Collections.Generic;
using Machine;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterJumpState : State
    {
        public override StateCategory Category { get; } = StateCategory.Jump;

        protected GameObject gameObject;
        protected AnimationClip jumpClip;
        protected CharacterAnimation characterAnimation;
        protected float jumpPower;
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetJumpClip(AnimationClip animationClip) => jumpClip = animationClip;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetJumpPower(float jumpPower) => this.jumpPower = jumpPower;
        
        
        public override void Initialize()
        {
            
        }

        public override void Update()
        {
        }

        public override void LateUpdate()
        {
        }
    }
    
    public class CharacterJumpStateBuilder : StateBuilder<CharacterJumpState>
    {
        public CharacterJumpStateBuilder(CharacterJumpState instance) : base(instance)
        {
        }
        
        public CharacterJumpStateBuilder SetGameObject(GameObject gameObject)
        {
            state.SetGameObject(gameObject);
            return this;
        }
        
        public CharacterJumpStateBuilder SetJumpClip(AnimationClip clip)
        {
            state.SetJumpClip(clip);
            return this;
        }
        
        public CharacterJumpStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            state.SetCharacterAnimation(characterAnimation);
            return this;
        }

        public CharacterJumpStateBuilder SetJumpPower(float value)
        {
            state.SetJumpPower(value);
            return this;
        }
    }
}