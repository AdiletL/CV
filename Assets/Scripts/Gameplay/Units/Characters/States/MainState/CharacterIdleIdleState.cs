using System;
using System.Collections.Generic;
using Calculate;
using Machine;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterIdleIdleState : UnitIdleState
    {
        public override StateCategory Category { get; } = StateCategory.idle;
        public CharacterAnimation CharacterAnimation { get; set; }
        public AnimationClip[] IdleClips { get; set; }

        protected AnimationClip getRandomIdleClip()
        {
            return IdleClips[UnityEngine.Random.Range(0, IdleClips.Length)];
        }

        public override void Enter()
        {
            base.Enter();
            CharacterAnimation.ChangeAnimation(getRandomIdleClip(), transitionDuration: .5f);
        }

    }

    public class CharacterIdleStateBuilder : UnitIdleStateBuilder
    {
        public CharacterIdleStateBuilder(CharacterIdleIdleState instance) : base(instance)
        {
        }

        public CharacterIdleStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if (state is CharacterIdleIdleState characterIdleIdleState)
            {
                characterIdleIdleState.CharacterAnimation = characterAnimation;
            }
            return this;
        }
        
        
        public CharacterIdleStateBuilder SetIdleClips(AnimationClip[] idleClips)
        {
            if (state is CharacterIdleIdleState characterIdleIdleState)
            {
                characterIdleIdleState.IdleClips = idleClips;
            }

            return this;
        }
    }
}