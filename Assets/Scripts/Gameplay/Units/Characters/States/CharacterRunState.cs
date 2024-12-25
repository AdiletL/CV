using UnityEngine;

namespace Unit.Character
{
    public class CharacterRunState : CharacterBaseMovementState
    {
        protected float durationAnimation;
        
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
            SetDurationAnimation();
            CharacterAnimation.ChangeAnimation(getRandomRunClip(), duration: durationAnimation);
        }

        protected void SetDurationAnimation()
        {
            durationAnimation = 1.5f / MovementSpeed;
            CharacterAnimation.SetSpeedAnimation(getRandomRunClip(), duration: durationAnimation);
        }

        public override void IncreaseMovementSpeed(float value)
        {
            base.IncreaseMovementSpeed(value);
            SetDurationAnimation();
        }

        public override void DecreaseMovementSpeed(float value)
        {
            base.DecreaseMovementSpeed(value);
            SetDurationAnimation();
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