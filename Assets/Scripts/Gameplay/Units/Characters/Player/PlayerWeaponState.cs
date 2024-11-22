using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerWeaponState : CharacterWeaponState
    {
    }

    public class PlayerWeaponBuilder : CharacterWeaponBuilder
    {
        public PlayerWeaponBuilder() : base(new PlayerWeaponState())
        {
        }
        
    }
}