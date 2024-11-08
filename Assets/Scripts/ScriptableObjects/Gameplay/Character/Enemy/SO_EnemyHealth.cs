using UnityEngine;

namespace ScriptableObjects.Character.Enemy
{
    [CreateAssetMenu(fileName = "SO_EnemyHealth", menuName = "SO/Gameplay/Character/Enemy/Health", order = 51)]
    public class SO_EnemyHealth : SO_CharacterHealth
    {
        [field: SerializeField, Space(5), Header("AnimationClip")]
        public AnimationClip takeDamageClip { get; private set; }
    }
}