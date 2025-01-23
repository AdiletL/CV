using UnityEngine;

namespace Unit.Character
{
    public class CharacterRunState : CharacterBaseMovementState
    {
        protected float durationAnimation;
        
        public CharacterSwitchAttack CharacterSwitchAttack { get; set; }
        public CharacterAnimation CharacterAnimation  { get; set; }
        public AnimationClip[] RunClips  { get; set; }

        protected AnimationClip getRandomRunClip()
        {
            return  RunClips[Random.Range(0, RunClips.Length)];
        }

        public override void Enter()
        {
            base.Enter();
            PlayAnimation();
        }

        protected void PlayAnimation()
        {
            UpdateDurationAnimation();
            CharacterAnimation.ChangeAnimationWithDuration(getRandomRunClip(), duration: durationAnimation);
        }

        protected void UpdateDurationAnimation()
        {
            durationAnimation = 1.5f / MovementSpeed;
            CharacterAnimation.SetSpeedClip(getRandomRunClip(), duration: durationAnimation);
        }

        public override void ExecuteMovement()
        {
            
        }

        public override void AddMovementSpeed(float value)
        {
            base.AddMovementSpeed(value);
            UpdateDurationAnimation();
        }

        public override void RemoveMovementSpeed(float value)
        {
            base.RemoveMovementSpeed(value);
            UpdateDurationAnimation();
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
        public CharacterRunStateBuilder SetCharacterSwitchAttack(CharacterSwitchAttack characterSwitchAttack)
        {
            if(state is CharacterRunState characterRunState)
                characterRunState.CharacterSwitchAttack = characterSwitchAttack;
            return this;
        }
    }
}