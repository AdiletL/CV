﻿using ScriptableObjects.Unit;
using UnityEngine;

namespace ScriptableObjects.Gameplay.Trap.Tower
{
    public abstract class SO_Tower : SO_Trap
    {
        [field: SerializeField] public int AmountAttack { get; protected set; }
        [field: SerializeField] public AnimationClip IdleClip { get; protected set; }
        [field: SerializeField] public AnimationClip AttackClip { get; protected set; }
    }
}