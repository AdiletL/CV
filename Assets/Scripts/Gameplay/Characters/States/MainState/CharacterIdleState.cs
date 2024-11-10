using System;
using System.Collections.Generic;
using Calculate;
using Machine;
using UnityEngine;

namespace Character
{
    public class CharacterIdleState : State
    {
        public override StateCategory Category { get; } = StateCategory.idle;
        
        public CharacterAnimation CharacterAnimation { get; set; }
        public AnimationClip IdleClip { get; set; }

        public override void Initialize()
        {
            
        }

        public override void Enter()
        {
            CharacterAnimation.ChangeAnimation(IdleClip, transitionDuration: .5f);
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
            
        }

       
    }

    public class CharacterIdleStateBuilder : StateBuilder<CharacterIdleState>
    {
        public CharacterIdleStateBuilder(CharacterIdleState instance) : base(instance)
        {
        }

        public CharacterIdleStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            state.CharacterAnimation = characterAnimation;
            return this;
        }
        
        
        public CharacterIdleStateBuilder SetIdleClip(AnimationClip idleClip)
        {
            state.IdleClip = idleClip;
            return this;
        }
    }
}