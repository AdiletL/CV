using UnityEngine;

namespace ScriptableObjects.Unit
{
    public abstract class SO_UnitEndurance : ScriptableObject
    {
        [field: SerializeField] public float MaxEndurance { get; protected set; } = 1;
    }
}