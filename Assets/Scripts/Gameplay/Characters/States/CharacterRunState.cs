using UnityEngine;

namespace Character
{
    public class CharacterRunState : CharacterBaseMovementState
    {
        public CharacterAnimation CharacterAnimation  { get; set; }
        public AnimationClip AnimationClip  { get; set; }

        public override void Enter()
        {
            var duration = 3 / MovementSpeed;
            CharacterAnimation.ChangeAnimation(AnimationClip, duration: duration);
        }

    }

    public class CharacterRunStateBuilder : CharacterBaseMovementStateBuilder
    {
        public CharacterRunStateBuilder(CharacterBaseMovementState instance) : base(instance)
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