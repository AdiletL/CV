using UnityEngine;

namespace ScriptableObjects.Unit
{
    public class SO_UnitAttack : ScriptableObject
    {
        [field: SerializeField] public int Damage { get; protected set; }
        [field: SerializeField] public DamageType DamageTypeID { get; protected set; } = DamageType.Physical;
        [field: SerializeField, Header("100 = 1 second"), Range(10, 400)] public int AttackSpeed { get; protected set; } = 50;
        [field: SerializeField, Range(10, 500)] public int AngleToTarget { get; protected set; } = 100;
        [field: SerializeField] public LayerMask EnemyLayer { get; protected set; }
    }
}