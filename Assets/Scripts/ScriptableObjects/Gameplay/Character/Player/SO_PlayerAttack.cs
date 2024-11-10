using UnityEngine;

namespace ScriptableObjects.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerAttack", menuName = "SO/Gameplay/Character/Player/Attack", order = 51)]
    public class SO_PlayerAttack: SO_CharacterAttack
    {
        [field: SerializeField, Space(5), Header("AnimationClips")] 
        public AnimationClip MeleeAttackClip { get; private set; }
        [field: SerializeField] public AnimationClip CooldownMeleeAttackClip { get; private set; }
    }
}