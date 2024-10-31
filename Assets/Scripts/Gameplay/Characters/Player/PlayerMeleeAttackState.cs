using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerMeleeAttackState : CharacterMeleeAttackState
    {
       
        
    }

    public class PlayerMeleeAttackStateBuilder : CharacterMeleeAttackStateBuilder
    {
        public PlayerMeleeAttackStateBuilder() : base(new PlayerMeleeAttackState())
        {
        }
        
    }
}