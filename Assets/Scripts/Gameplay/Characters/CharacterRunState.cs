using UnityEngine;

namespace Character
{
    public class CharacterRunState : CharacterBaseMovementState
    {
        public CharacterAnimation CharacterAnimation;
        public AnimationClip AnimationClip;

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