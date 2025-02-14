using UnityEngine;

namespace Unit.Character
{
    public class CharacterRunState : CharacterBaseMovementState
    {
        protected UnitAnimation unitAnimation;
        protected UnitEndurance unitEndurance;
        protected AnimationClip[] runClips;

        protected float durationAnimation;
        protected int animationLayer;
        
        public void SetUnitAnimation(UnitAnimation unitAnimation) => this.unitAnimation = unitAnimation;
        public void SetUnitEndurance(UnitEndurance unitEndurance) => this.unitEndurance = unitEndurance;
        public void SetRunClips(AnimationClip[] runClips) => this.runClips = runClips;
        

        protected AnimationClip getRandomRunClip()
        {
            return  runClips[Random.Range(0, runClips.Length)];
        }

        public override void Enter()
        {
            base.Enter();
            PlayAnimation();
        }

        protected void PlayAnimation()
        {
            UpdateDurationAnimation();
            unitAnimation.ChangeAnimationWithDuration(getRandomRunClip(), duration: durationAnimation, layer: animationLayer);
        }

        protected void UpdateDurationAnimation()
        {
            durationAnimation = 1.5f / MovementSpeed;
            unitAnimation.SetSpeedClip(getRandomRunClip(), duration: durationAnimation);
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

        public CharacterRunStateBuilder SetUnitAnimation(UnitAnimation unitAnimation)
        {
            if(state is CharacterRunState characterRunState)
                characterRunState.SetUnitAnimation(unitAnimation);
            return this;
        }

        public CharacterRunStateBuilder SetRunClips(AnimationClip[] animationClips)
        {
            if(state is CharacterRunState characterRunState)
                characterRunState.SetRunClips(animationClips);
            return this;
        }

        public CharacterRunStateBuilder SetUnitEndurance(UnitEndurance unitEndurance)
        {
            if(state is CharacterRunState characterRunState)
                characterRunState.SetUnitEndurance(unitEndurance);
            return this;
        }
    }
}