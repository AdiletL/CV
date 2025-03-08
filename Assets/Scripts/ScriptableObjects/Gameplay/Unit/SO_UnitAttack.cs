using UnityEngine;

namespace ScriptableObjects.Unit
{
    public class SO_UnitAttack : ScriptableObject
    {
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField, Header("100 = 1 second"), Range(100, 500)] public int AttackSpeed { get; set; } = 100;
        [field: SerializeField, Range(10, 500)] public int AngleToTarget { get; set; } = 100;
        [field: SerializeField] public LayerMask EnemyLayer { get; set; }
    }
}