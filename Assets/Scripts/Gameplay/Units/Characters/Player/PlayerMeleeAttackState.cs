using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerMeleeAttackState : CharacterMeleeAttackState
    {
    }

    public class PlayerMeleeAttackBuilder : CharacterMeleeAttackBuilder
    {
        public PlayerMeleeAttackBuilder() : base(new PlayerMeleeAttackState())
        {
        }
        
    }
}