using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Skill
{
    public class SkillHandler : MonoBehaviour, IHandler
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;


        private Dictionary<Type, ISkill> currentSkills = new();


        public bool IsSkillActive(Type skill)
        {
            return currentSkills.ContainsKey(skill);
        }

        public bool IsSkillNotNull(Type skill)
        {
            return currentSkills.ContainsKey(skill);
        }

        public ISkill GetSkill(Type skill)
        {
            return currentSkills[skill];
        }

        public void Initialize()
        {

        }

        private void Update() => OnUpdate?.Invoke();

        private void LateUpdate() => OnLateUpdate?.Invoke();

        public void Execute(Type skillType, Action exitCallback = null)
        {
            if (currentSkills.ContainsKey(skillType))
            {
                var skill = currentSkills[skillType];
                OnUpdate += skill.Update;
                OnLateUpdate += skill.LateUpdate;
                skill.OnFinished += OnFinished;
                skill.Execute(exitCallback);
            }
        }

        private void OnFinished(ISkill skill)
        {
            OnUpdate -= skill.Update;
            OnLateUpdate -= skill.LateUpdate;
            skill.OnFinished -= OnFinished;
        }

        public void AddSkill(ISkill skill)
        {
            if (!currentSkills.ContainsKey(skill.GetType()))
            {
                currentSkills.Add(skill.GetType(), skill);
            }
        }

        public void RemoveSkill(ISkill skill)
        {
            if (currentSkills.ContainsKey(skill.GetType()))
            {
                OnFinished(skill);
                currentSkills.Remove(skill.GetType());
            }
        }
    }
}