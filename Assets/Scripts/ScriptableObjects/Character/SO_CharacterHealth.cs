using UnityEngine;

namespace ScriptableObjects.Character
{
    public abstract class SO_CharacterHealth : ScriptableObject
    {
        [field: SerializeField] public int MaxHealth { get; set; }
    }
}