using System;
using Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Skill
{
    [Flags]
    public enum SkillType
    {
        nothing,
        dash = 1 << 0,
        spawnTeleport = 1 << 1,
    }
    
    public abstract class Skill : ISkill
    {
        [Inject] protected DiContainer diContainer;
        public event Action<ISkill> OnFinished;

        public GameObject GameObject { get; protected set; }
        public abstract SkillType SkillType { get; protected set; }
        public InputType BlockedInputType { get; protected set; }
        public SkillType BlockedSkillType { get; protected set; }
        public Action ExitCallBack { get; protected set; }
        public AnimationClip CastClip { get; protected set; }
        public bool IsCanUseSkill { get; protected set; }
        

        public void SetGameObject(GameObject gameObject) => this.GameObject = gameObject;
        public void SetBlockedInputType(InputType inputType) => this.BlockedInputType = inputType;
        public void SetBlockedSkillType(SkillType skillType) => this.BlockedSkillType = skillType;
        public void SetCastClip(AnimationClip clip) => this.CastClip = clip;
        

        public virtual void Initialize()
        {
        }

        public virtual void Execute(Action exitCallBack = null)
        {
            ExitCallBack = exitCallBack;
        }

        public abstract void Update();

        public abstract void LateUpdate();
        public abstract void CheckTarget();

        public virtual void Exit()
        {
            IsCanUseSkill = false;
            ExitCallBack?.Invoke();
            OnFinished?.Invoke(this);
        }
    }
    

    public abstract class SkillBuilder<T> where T : Skill
    {
        protected Skill skill;

        public SkillBuilder(Skill instance)
        {
            skill = instance;
        }

        public SkillBuilder<T> SetGameObject(GameObject gameObject)
        {
            skill.SetGameObject(gameObject);
            return this;
        }
        public SkillBuilder<T> SetBlockedInputType(InputType inputType)
        {
            skill.SetBlockedInputType(inputType);
            return this;
        }
        public SkillBuilder<T> SetBlockedSkillType(SkillType skillType)
        {
            skill.SetBlockedSkillType(skillType);
            return this;
        }
        
        public Skill Build()
        {
            return skill;
        }
    }
}