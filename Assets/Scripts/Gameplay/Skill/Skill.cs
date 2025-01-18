using System;
using UnityEngine;
using Zenject;

namespace Gameplay.Skill
{
    public abstract class Skill : ISkill
    {
        public event Action<ISkill> OnExit;

        protected DiContainer diContainer;
        public GameObject GameObject { get; set; }
        public Action ExitCallBack { get; protected set; }
        public AnimationClip CastClip { get; protected set; }

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }

        public virtual void Initialize(AnimationClip castClip = null)
        {
            CastClip = castClip;
        }

        public virtual void Execute(Action exitCallBack = null)
        {
            ExitCallBack = exitCallBack;
        }

        public abstract void Update();

        public abstract void LateUpdate();

        public virtual void Exit()
        {
            ExitCallBack?.Invoke();
            OnExit?.Invoke(this);
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
            skill.GameObject = gameObject;
            return this;
        }
        
        public Skill Build()
        {
            return skill;
        }
    }
}