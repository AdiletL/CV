using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerMeleeAttackState : CharacterMeleeAttackState
    {
        protected override int checkEnemyLayer { get; } = Layers.ENEMY_LAYER;
    }

    public class PlayerMeleeAttackBuilder : CharacterMeleeAttackBuilder
    {
        public PlayerMeleeAttackBuilder() : base(new PlayerMeleeAttackState())
        {
        }
        
    }
}