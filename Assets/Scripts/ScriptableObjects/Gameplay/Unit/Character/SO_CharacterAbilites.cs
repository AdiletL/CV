using Gameplay.Ability;
using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterAbilites : ScriptableObject
    {
        [field: SerializeField] public AbilityType[] AbilityTypesID { get; protected set; }
    }
}