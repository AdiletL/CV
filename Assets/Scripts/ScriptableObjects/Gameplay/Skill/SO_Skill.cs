using Gameplay.Skill;
using Unit.Character.Player;
using UnityEngine;

namespace ScriptableObjects.Gameplay.Skill
{
    public abstract class SO_Skill : ScriptableObject
    {
        [field: SerializeField, Space] public InputType BlockedInputType { get; private set; }
        [field: SerializeField, Space] public SkillType BlockedSkillType { get; private set; }
    }
}