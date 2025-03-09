using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public class CharacterMoveState : State, IMovement
    {
        public override StateCategory Category { get; } = StateCategory.Move;
        
        protected SO_CharacterMove so_CharacterMove;
        protected UnitAnimation unitAnimation;
        protected UnitEndurance unitEndurance;
        protected GameObject gameObject;
        protected Transform center;
        protected AnimationClip[] runClips;
        protected AnimationClip currentClip;

        protected float durationAnimation;
        protected const string SPEED_MOVEMENT_NAME = "SpeedMovement";
        
        public Stat MovementSpeedStat { get; } = new Stat();


        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetCenter(Transform center) => this.center = center;
        public void SetConfig(SO_CharacterMove so_CharacterMove) => this.so_CharacterMove = so_CharacterMove;
        public void SetUnitAnimation(UnitAnimation unitAnimation) => this.unitAnimation = unitAnimation;
        public void SetUnitEndurance(UnitEndurance unitEndurance) => this.unitEndurance = unitEndurance;
        

        protected AnimationClip getRandomClip(AnimationClip[] clips)
        {
            return clips[Random.Range(0, clips.Length)];
        }

        public override void Initialize()
        {
            base.Initialize();
            MovementSpeedStat.AddValue(so_CharacterMove.RunSpeed);
            runClips = so_CharacterMove.RunClip;
            unitAnimation.AddClips(runClips);
        }

        public override void Update()
        {
            
        }

        public override void LateUpdate()
        {
            
        }

        protected void PlayAnimation()
        {
            UpdateDurationAnimation();
            unitAnimation.ChangeAnimationWithDuration(currentClip, duration: durationAnimation, SPEED_MOVEMENT_NAME);
        }

        protected void UpdateDurationAnimation()
        {
            durationAnimation = 1.5f / MovementSpeedStat.CurrentValue;
            unitAnimation.SetSpeedClip(currentClip, duration: durationAnimation, SPEED_MOVEMENT_NAME);
        }
        
        public virtual void ExecuteMovement()
        {
            
        }
    }

    public class CharacterMoveStateBuilder : StateBuilder<CharacterMoveState>
    {
        public CharacterMoveStateBuilder(CharacterMoveState instance) : base(instance)
        {
        }

        public CharacterMoveStateBuilder SetGameObject(GameObject gameObject)
        {
            state.SetGameObject(gameObject);
            return this;
        }
        public CharacterMoveStateBuilder SetCenter(Transform center)
        {
            state.SetCenter(center);
            return this;
        }
        public CharacterMoveStateBuilder SetConfig(SO_CharacterMove config)
        {
            state.SetConfig(config);
            return this;
        }
        public CharacterMoveStateBuilder SetUnitAnimation(UnitAnimation unitAnimation)
        {
            state.SetUnitAnimation(unitAnimation);
            return this;
        }
        public CharacterMoveStateBuilder SetUnitEndurance(UnitEndurance unitEndurance)
        {
            state.SetUnitEndurance(unitEndurance);
            return this;
        }
    }
}