using UnityEngine;

namespace ScriptableObjects.Character
{
    public class SO_CharacterAttack : ScriptableObject
    {
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField] public float AmountAttack { get; set; }
    }
}