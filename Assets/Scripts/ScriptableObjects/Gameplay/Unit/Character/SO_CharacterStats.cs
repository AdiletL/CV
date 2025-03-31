using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterStats : SerializedScriptableObject
    {
        [field: SerializeField, Range(0, 1f), Tooltip("Percent")] public float EvasionChance { get; private set; }
        [field: SerializeField] public float Armor { get; private set; }
        [field: SerializeField] public float MagicalResistance { get; private set; }
        [field: SerializeField] public float PureResistance { get; private set; }
    }
}