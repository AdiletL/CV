using ScriptableObjects.Gameplay.Equipment;
using UnityEngine;

namespace ScriptableObjects.Gameplay.Equipment.Weapon
{
    public abstract class SO_Weapon : SO_Equipment
    {
        [field: SerializeField] public float AngleToTarget { get; protected set; }
    }
}