using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Skill
{
    public class SkillHandler : MonoBehaviour, IHandler
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;


        private Dictionary<SkillType, ISkill> currentSkills = new();


        public bool IsSkillActive(SkillType skillType)
        {
            return currentSkills.ContainsKey(skillType);
        }

        public bool IsSkillNotNull(SkillType skillType)
        {
            return currentSkills.ContainsKey(skillType);
        }

        public ISkill GetSkill(SkillType skillType)
        {
            return currentSkills[skillType];
        }

        public void Initialize()
        {

        }

        private void Update() => OnUpdate?.Invoke();

        private void LateUpdate() => OnLateUpdate?.Invoke();

        public void Execute(SkillType skillType, Action exitCallback = null)
        {
            if (IsSkillNotNull(skillType))
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
            if (!IsSkillNotNull(skill.SkillType))
            {
                currentSkills.Add(skill.SkillType, skill);
            }
        }

        public void RemoveSkill(SkillType skillType)
        {
            if (IsSkillNotNull(skillType))
            {
                OnFinished(currentSkills[skillType]);
                currentSkills.Remove(skillType);
            }
        }
    }
}