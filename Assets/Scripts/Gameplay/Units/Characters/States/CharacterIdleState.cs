using System;
using System.Collections.Generic;
using Calculate;
using Machine;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterIdleState : UnitIdleState
    {
        public override StateCategory Category { get; } = StateCategory.idle;

        protected CharacterAnimation characterAnimation;
        protected AnimationClip[] IdleClips;
        
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetIdleClips(AnimationClip[] idleClips) => IdleClips = idleClips;
        

        protected AnimationClip getRandomIdleClip()
        {
            return IdleClips[UnityEngine.Random.Range(0, IdleClips.Length)];
        }

        public override void Enter()
        {
            base.Enter();
            PlayAnimation();
        }

        protected void PlayAnimation()
        {
            characterAnimation.ChangeAnimationWithDuration(getRandomIdleClip(), transitionDuration: .5f);
        }
    }

    public class CharacterIdleStateBuilder : UnitIdleStateBuilder
    {
        public CharacterIdleStateBuilder(CharacterIdleState instance) : base(instance)
        {
        }

        public CharacterIdleStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if (state is CharacterIdleState characterIdleIdleState)
                characterIdleIdleState.SetCharacterAnimation(characterAnimation);
            return this;
        }
        
        
        public CharacterIdleStateBuilder SetIdleClips(AnimationClip[] idleClips)
        {
            if (state is CharacterIdleState characterIdleIdleState)
                characterIdleIdleState.SetIdleClips(idleClips);
            return this;
        }
    }
}