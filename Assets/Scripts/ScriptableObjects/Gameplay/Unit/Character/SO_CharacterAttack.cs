using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterAttack : ScriptableObject
    {
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField] public int AmountAttack { get; set; }
        [field: SerializeField] public float Range { get; set; }
    }
}