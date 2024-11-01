﻿using System;
using System.Collections.Generic;
using Calculate;
using UnityEngine;

namespace Character
{
    public class CharacterIdleState : State
    {
        public CharacterAnimation CharacterAnimation;
        public AnimationClip IdleClip;
        
        public override void Enter()
        {
            CharacterAnimation.ChangeAnimation(IdleClip);
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