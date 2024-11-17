using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterAttack : ScriptableObject
    {
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField] public float AmountAttack { get; set; }
        [field: SerializeField] public float RangeAttack { get; set; }
    }
}