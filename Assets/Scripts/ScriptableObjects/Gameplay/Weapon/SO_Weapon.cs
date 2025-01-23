using UnityEngine;

namespace ScriptableObjects.Weapon
{
    public abstract class SO_Weapon : ScriptableObject
    {
        [field: SerializeField] public GameObject WeaponPrefab { get; set; }
        [field: SerializeField] public int Damage { get; protected set; }
        [field: SerializeField] public float Range { get; protected set; }
        [field: SerializeField] public float AngleToTarget { get; protected set; }
        
        [field: SerializeField] public EnduranceInfo ReductionEndurance { get; protected set; }
        [field: SerializeField] public AttackSpeedInfo IncreaseAttackSpeed { get; protected set; }
    }
}