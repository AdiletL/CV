using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterSkills : ScriptableObject
    {
        [field: SerializeField] public SkillType SkillTypeID { get; protected set; }
    }
}