using Gameplay.Ability;
using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterSkills : ScriptableObject
    {
        [field: SerializeField] public AbilityConfigData AbilityConfigData { get; protected set; }
    }
}