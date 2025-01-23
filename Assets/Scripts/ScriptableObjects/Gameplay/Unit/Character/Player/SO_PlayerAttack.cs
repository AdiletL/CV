using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerAttack", menuName = "SO/Gameplay/Unit/Character/Player/Attack", order = 51)]
    public class SO_PlayerAttack: SO_CharacterAttack
    {
        [field: SerializeField, Space(10), Header("Default")] 
        public AnimationClip[] DefaultAttackClips { get; private set; }
        [field: SerializeField] public AnimationClip DefaultCooldownClip { get; private set; }
        
        
        [field: SerializeField, Space(10), Header("SwordClips")] 
        public AnimationClip[] SwordAttackClip { get; private set; }
        [field: SerializeField] public AnimationClip SwordCooldownClip { get; private set; }
        
        
        [field: SerializeField, Space(10), Header("BowClips")] 
        public AnimationClip[] BowAttackClip { get; private set; }
        [field: SerializeField] public AnimationClip BowCooldownClip { get; private set; }
        
    }
}