using UnityEngine;

namespace ScriptableObjects.Gameplay.Trap
{
    public abstract class SO_Trap : ScriptableObject
    {
        [field: SerializeField] public int Damage { get; protected set; }
    }
}