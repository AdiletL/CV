﻿using UnityEngine;

namespace Character
{
    public class CharacterRunState : CharacterBaseMovementState
    {
        public CharacterAnimation CharacterAnimation  { get; set; }
        public AnimationClip AnimationClip  { get; set; }

        public override void Enter()
        {
            CharacterAnimation.ChangeAnimation(AnimationClip);
        }

    }

    public class CharacterBaseRunStateBuilder : CharacterBaseMovementStateBuilder
    {
        public CharacterBaseRunStateBuilder(CharacterBaseMovementState instance) : base(instance)
        {
        }

        public CharacterBaseRunStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if(state is CharacterRunState characterRunState)
                characterRunState.CharacterAnimation = characterAnimation;
            return this;
        }

        public CharacterBaseRunStateBuilder SetAnimationClip(AnimationClip animationClip)
        {
            if(state is CharacterRunState characterRunState)
                characterRunState.AnimationClip = animationClip;
            return this;
        }
    }
}