using UnityEngine;

namespace ScriptableObjects.Unit
{
    public abstract class SO_UnitMana : ScriptableObject
    {
        [field: SerializeField] public int MaxMana { get; private set; }
    }
}