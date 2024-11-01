﻿using UnityEngine;

namespace ScriptableObjects.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerAttack", menuName = "SO/Character/Player/Attack", order = 51)]
    public class SO_PlayerAttack: SO_CharacterAttack
    {
        [field: SerializeField, Space(5), Header("AnimationClips")] 
        public AnimationClip MeleeAttackClip { get; private set; }
    }
}