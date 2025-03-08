using System.Collections.Generic;
using Unit.Character.Player;
using UnityEngine;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerAttack", menuName = "SO/Gameplay/Unit/Character/Player/Attack", order = 51)]
    public class SO_PlayerAttack: SO_CharacterAttack
    {
        [field: SerializeField, Space(10), Header("SwordClips")] 
        public AnimationClip[] SwordAttackClip { get; private set; }
        [field: SerializeField] public AnimationClip SwordCooldownClip { get; private set; }
        
        
        [field: SerializeField, Space(10), Header("BowClips")] 
        public AnimationClip[] BowAttackClip { get; private set; }
        [field: SerializeField] public AnimationClip BowCooldownClip { get; private set; }

        [field: SerializeField, Space(10)] public float RotationSpeed { get; private set; } = 1000;
        
        [field: SerializeField, Space] public InputType BlockInputType { get; private set; }
    }
}