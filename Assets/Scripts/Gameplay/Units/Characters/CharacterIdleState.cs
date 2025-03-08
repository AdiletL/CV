using System;
using System.Collections.Generic;
using Calculate;
using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public class CharacterIdleState : UnitIdleState
    {
        public override StateCategory Category { get; } = StateCategory.Idle;

        protected SO_CharacterMove so_CharacterMove;
        protected CharacterAnimation characterAnimation;
        protected AnimationClip[] idleClips;
        
        public void SetConfig(SO_CharacterMove so_CharacterMove) => this.so_CharacterMove = so_CharacterMove;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        

        protected AnimationClip getRandomIdleClip()
        {
            return idleClips[UnityEngine.Random.Range(0, idleClips.Length)];
        }

        public override void Initialize()
        {
            base.Initialize();
            idleClips = so_CharacterMove.IdleClip;
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

        public CharacterIdleStateBuilder SetConfig(SO_CharacterMove config)
        {
            if (state is CharacterIdleState characterIdleIdleState)
                characterIdleIdleState.SetConfig(config);
            return this;
        }
        public CharacterIdleStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if (state is CharacterIdleState characterIdleIdleState)
                characterIdleIdleState.SetCharacterAnimation(characterAnimation);
            return this;
        }
    }
}