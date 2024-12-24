using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterAttack : SO_UnitAttack
    {
        [field: SerializeField] public float Range { get; set; }
    }
}