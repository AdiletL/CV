using UnityEngine;

namespace ScriptableObjects.Unit.Character.Creep
{
    [CreateAssetMenu(fileName = "SO_EnemyHealth", menuName = "SO/Gameplay/Character/Creep/Health", order = 51)]
    public class SO_CreepHealth : SO_CharacterHealth
    {
        [field: SerializeField, Space(5), Header("AnimationClip")]
        public AnimationClip takeDamageClip { get; private set; }
    }
}