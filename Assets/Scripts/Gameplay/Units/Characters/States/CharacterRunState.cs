using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterRunState : CharacterBaseMovementState
    {
        protected SO_CharacterMove so_CharacterMove;
        protected UnitAnimation unitAnimation;
        protected UnitEndurance unitEndurance;
        protected AnimationClip[] runClips;

        protected float durationAnimation;
        protected const string SPEED_MOVEMENT_NAME = "SpeedMovement";
        
        public void SetConfig(SO_CharacterMove so_CharacterMove) => this.so_CharacterMove = so_CharacterMove;
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
            unitAnimation.ChangeAnimationWithDuration(getRandomRunClip(), duration: durationAnimation, SPEED_MOVEMENT_NAME);
        }

        protected void UpdateDurationAnimation()
        {
            durationAnimation = 1.5f / MovementSpeedStat.CurrentValue;
            unitAnimation.SetSpeedClip(getRandomRunClip(), duration: durationAnimation, SPEED_MOVEMENT_NAME);
        }

        public override void ExecuteMovement()
        {
            
        }
    }

    public class CharacterRunStateBuilder : CharacterBaseMovementStateBuilder
    {
        public CharacterRunStateBuilder(CharacterBaseMovementState instance) : base(instance)
        {
        }

        public CharacterRunStateBuilder SetConfig(SO_CharacterMove config)
        {
            if(state is CharacterRunState characterRunState)
                characterRunState.SetConfig(config);
            return this;
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