using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
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