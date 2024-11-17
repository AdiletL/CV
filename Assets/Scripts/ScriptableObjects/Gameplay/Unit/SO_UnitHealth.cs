using UnityEngine;

namespace ScriptableObjects.Unit
{
    public abstract class SO_UnitHealth : ScriptableObject
    {
        [field: SerializeField] public int MaxHealth { get; set; }
    }
}