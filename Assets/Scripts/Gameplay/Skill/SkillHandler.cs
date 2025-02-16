using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Skill
{
    public class SkillHandler : MonoBehaviour, IHandler
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;


        private Dictionary<SkillType, List<ISkill>> currentSkills = new();


        public bool IsSkillActive(SkillType skillType)
        {
            return currentSkills.ContainsKey(skillType);
        }

        public bool IsSkillNotNull(SkillType skillType)
        {
            return currentSkills.ContainsKey(skillType);
        }

        public ISkill GetSkill(SkillType skillType, int id)
        {
            for (int i = currentSkills[skillType].Count - 1; i >= 0; i--)
            {
                if (currentSkills[skillType][i].ID == id)
                    return currentSkills[skillType][i];
            }

            return null;
        }
        
        public List<ISkill> GetSkills(SkillType skillType)
        {
            return currentSkills[skillType];
        }

        public void Initialize()
        {

        }

        private void Update() => OnUpdate?.Invoke();
        private void LateUpdate() => OnLateUpdate?.Invoke();
        

        public void Execute(SkillType skillType, int id, Action exitCallback = null)
        {
            if (IsSkillNotNull(skillType))
            {
                for (int i = currentSkills[skillType].Count - 1; i >= 0; i--)
                {
                    if(currentSkills[skillType][i].ID != id) continue;
                    
                    OnUpdate += currentSkills[skillType][i].Update;
                    OnLateUpdate += currentSkills[skillType][i].LateUpdate;
                    currentSkills[skillType][i].OnFinished += OnFinished;
                    currentSkills[skillType][i].Execute(exitCallback);
                    break;
                }
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
                currentSkills.Add(skill.SkillType, new List<ISkill>());
            
            currentSkills[skill.SkillType].Add(skill);
        }

        public void RemoveSkillByID(SkillType skillType, int id)
        {
            if (IsSkillNotNull(skillType))
            {
                for (int i = currentSkills[skillType].Count - 1; i >= 0; i--)
                {
                    if (currentSkills[skillType][i].ID != id) continue;
                    OnFinished(currentSkills[skillType][i]);
                    currentSkills[skillType].Remove(currentSkills[skillType][i]);
                    break;
                }
            }
        }

        public void ExitSkillByID(SkillType skillType, int id)
        {
            if (IsSkillNotNull(skillType))
            {
                for (int i = currentSkills[skillType].Count - 1; i >= 0; i--)
                {
                    if (currentSkills[skillType][i].ID != id) continue;
                    currentSkills[skillType][i].Exit();
                    break;
                }
            }
        }
    }
}