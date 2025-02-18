using Gameplay.Ability;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterSpecialAction : ScriptableObject
    {
        [field: SerializeField] public AbilityConfigData AbilityConfigData { get; protected set; }
    }
}