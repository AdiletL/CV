
using UnityEngine;

namespace ScriptableObjects.Unit
{
    public class SO_UnitAttack : ScriptableObject
    {
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField] public float AmountAttackInSecond { get; set; }
        [field: SerializeField] public LayerMask EnemyLayer { get; set; }
    }
}