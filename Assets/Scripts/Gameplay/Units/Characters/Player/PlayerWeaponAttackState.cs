using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerWeaponAttackState : CharacterWeaponAttackState
    {

    }

    public class PlayerWeaponAttackStateStateBuilder : CharacterWeaponAttackStateBuilder
    {
        public PlayerWeaponAttackStateStateBuilder() : base(new PlayerWeaponAttackState())
        {
        }
    }
}