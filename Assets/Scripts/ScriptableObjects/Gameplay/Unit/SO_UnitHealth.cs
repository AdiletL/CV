using UnityEngine;

namespace ScriptableObjects.Unit
{
    public abstract class SO_UnitHealth : ScriptableObject
    {
        [field: SerializeField] public int MaxHealth { get; protected set; }
        [field: SerializeField] public float RegenerationHealthRate { get; protected set; }
        [field: SerializeField, Space] public bool IsCanTakeDamageEffect { get; protected set; } = true;
    }
}