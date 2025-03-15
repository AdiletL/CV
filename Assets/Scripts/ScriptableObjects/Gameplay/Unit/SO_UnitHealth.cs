using UnityEngine;

namespace ScriptableObjects.Unit
{
    public abstract class SO_UnitHealth : ScriptableObject
    {
        [field: SerializeField] public int MaxHealth { get; protected set; }
        [field: SerializeField, Header("In second 10hp = 1")] public int RegenerationHealth { get; protected set; }
        [field: SerializeField, Space] public bool IsCanTakeDamageEffect { get; protected set; } = true;
    }
}