
using UnityEngine;

namespace ScriptableObjects.Unit
{
    public class SO_UnitAttack : ScriptableObject
    {
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField] public int AmountAttack { get; set; }
    }
}