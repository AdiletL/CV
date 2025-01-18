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
        
        public CharacterSwitchMove CharacterSwitchMove { get; set; }
        public CharacterSwitchAttack CharacterSwitchAttack { get; set; }
        public CharacterController CharacterController { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public AnimationClip[] IdleClips { get; set; }
        

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
            CharacterAnimation.ChangeAnimationWithDuration(getRandomIdleClip(), transitionDuration: .5f);
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
            {
                characterIdleIdleState.CharacterAnimation = characterAnimation;
            }
            return this;
        }
        
        
        public CharacterIdleStateBuilder SetIdleClips(AnimationClip[] idleClips)
        {
            if (state is CharacterIdleState characterIdleIdleState)
            {
                characterIdleIdleState.IdleClips = idleClips;
            }

            return this;
        }
        
        public CharacterIdleStateBuilder SetCharacterSwitchMove(CharacterSwitchMove characterSwitchMove)
        {
            if (state is CharacterIdleState characterIdleIdleState)
            {
                characterIdleIdleState.CharacterSwitchMove = characterSwitchMove;
            }

            return this;
        }
        public CharacterIdleStateBuilder SetCharacterSwitchAttack(CharacterSwitchAttack characterSwitchAttack)
        {
            if (state is CharacterIdleState characterIdleIdleState)
            {
                characterIdleIdleState.CharacterSwitchAttack = characterSwitchAttack;
            }

            return this;
        }
        public CharacterIdleStateBuilder SetCharacterController(CharacterController characterController)
        {
            if (state is CharacterIdleState characterIdleIdleState)
            {
                characterIdleIdleState.CharacterController = characterController;
            }

            return this;
        }
    }
}