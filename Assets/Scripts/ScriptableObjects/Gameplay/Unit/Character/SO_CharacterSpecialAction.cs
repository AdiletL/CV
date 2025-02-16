using Gameplay.Skill;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterSpecialAction : ScriptableObject
    {
        [field: SerializeField] public SkillConfigData SkillConfigData { get; protected set; }
    }
}