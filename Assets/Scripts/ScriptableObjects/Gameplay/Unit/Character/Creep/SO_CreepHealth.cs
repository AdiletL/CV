using UnityEngine;

namespace ScriptableObjects.Unit.Character.Creep
{
    public class SO_CreepHealth : SO_CharacterHealth
    {
        [field: SerializeField, Space(5), Header("AnimationClip")]
        public AnimationClip takeDamageClip { get; private set; }
    }
}