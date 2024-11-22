using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerAttack", menuName = "SO/Gameplay/Character/Player/Attack", order = 51)]
    public class SO_PlayerAttack: SO_CharacterAttack
    {
        [field: SerializeField, Space(5), Header("WeaponClips")] 
        public List<AnimationClip> SwordAttackClip { get; private set; }
        [field: SerializeField] public AnimationClip SwordCooldownClip { get; private set; }
    }
}