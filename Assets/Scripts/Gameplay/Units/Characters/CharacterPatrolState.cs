using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public class CharacterPatrolState : State, IPatrol
    {
        public override StateCategory Category { get; } = StateCategory.Move;
        
        protected SO_CharacterMove so_CharacterMove;
        protected CharacterAnimation characterAnimation;
        protected CharacterEndurance characterEndurance;
        protected GameObject gameObject;
        protected Transform center;
        protected AnimationClip[] walkClips;
        protected AnimationClip currentClip;
        
        protected float durationAnimation;
        protected const string SPEED_MOVEMENT_NAME = "SpeedMovement";
        
        public Vector3[] PatrolPoints { get; protected set; }
        public Stat MovementSpeedStat { get; } = new Stat();
        
        public void SetPatrolPoints(Vector3[] points) => PatrolPoints = points;
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetCenter(Transform center) => this.center = center;
        public void SetConfig(SO_CharacterMove so_CharacterMove) => this.so_CharacterMove = so_CharacterMove;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetCharacterEndurance(CharacterEndurance characterEndurance) => this.characterEndurance = characterEndurance;

        
        protected AnimationClip getRandomClip(AnimationClip[] clips)
        {
            return clips[Random.Range(0, clips.Length)];
        }
        

        public override void Enter()
        {
            base.Enter();
            currentClip = getRandomClip(walkClips);
            PlayAnimation();
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
            characterAnimation.ChangeAnimationWithDuration(currentClip, duration: durationAnimation, SPEED_MOVEMENT_NAME);
        }

        protected void UpdateDurationAnimation()
        {
            durationAnimation = 1.5f / MovementSpeedStat.CurrentValue;
            characterAnimation.SetSpeedClip(currentClip, duration: durationAnimation, SPEED_MOVEMENT_NAME);
        }
        
        public virtual void ExecuteMovement()
        {
            
        }
    }

    public class CharacterPatrolStateBuilder : StateBuilder<CharacterPatrolState>
    {
        public CharacterPatrolStateBuilder(CharacterPatrolState instance) : base(instance)
        {
        }
        
        public CharacterPatrolStateBuilder SetGameObject(GameObject gameObject)
        {
            state.SetGameObject(gameObject);
            return this;
        }
        public CharacterPatrolStateBuilder SetCenter(Transform center)
        {
            state.SetCenter(center);
            return this;
        }
        public CharacterPatrolStateBuilder SetConfig(SO_CharacterMove config)
        {
            state.SetConfig(config);
            return this;
        }
        public CharacterPatrolStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            state.SetCharacterAnimation(characterAnimation);
            return this;
        }
        public CharacterPatrolStateBuilder SetCharacterEndurance(CharacterEndurance characterEndurance)
        {
            state.SetCharacterEndurance(characterEndurance);
            return this;
        }
        public CharacterPatrolStateBuilder SetPatrolPoints(Vector3[] points)
        {
            state.SetPatrolPoints(points);
            return this;
        }
    }
}