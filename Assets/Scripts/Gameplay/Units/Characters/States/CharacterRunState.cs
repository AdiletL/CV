using UnityEngine;

namespace Unit.Character
{
    public class CharacterRunState : CharacterBaseMovementState
    {
        public CharacterAnimation CharacterAnimation  { get; set; }
        public AnimationClip[] RunClips  { get; set; }

        protected AnimationClip getRandomRunClip()
        {
            return  RunClips[Random.Range(0, RunClips.Length)];
        }

        public override void Enter()
        {
            PlayAnimation();
        }

        protected void PlayAnimation()
        {
            var duration = 2 / MovementSpeed;
            CharacterAnimation.ChangeAnimation(getRandomRunClip(), duration: duration);
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

        public CharacterRunStateBuilder SetRunClips(AnimationClip[] animationClips)
        {
            if(state is CharacterRunState characterRunState)
                characterRunState.RunClips = animationClips;
            return this;
        }
    }
}