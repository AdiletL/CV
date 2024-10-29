using UnityEngine;

namespace Character
{
    public class CharacterRunState : CharacterMovementState
    {
        public CharacterAnimation CharacterAnimation;
        public AnimationClip AnimationClip;

        public override void Enter()
        {
            CharacterAnimation.ChangeAnimation(AnimationClip);
        }

    }

    public class CharacterRunStateBuilder : CharacterMovementStateBuilder
    {
        public CharacterRunStateBuilder(CharacterMovementState instance) : base(instance)
        {
        }

        public CharacterRunStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if(state is CharacterRunState characterRunState)
                characterRunState.CharacterAnimation = characterAnimation;
            return this;
        }

        public CharacterRunStateBuilder SetAnimationClip(AnimationClip animationClip)
        {
            if(state is CharacterRunState characterRunState)
                characterRunState.AnimationClip = animationClip;
            return this;
        }
    }
}