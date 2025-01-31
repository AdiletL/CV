﻿using Machine;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterTakeDamageState : State
    {
        public override StateCategory Category { get; } = StateCategory.action;

        public override void Initialize()
        {
            
        }


        public override void LateUpdate()
        {
            
        }
        public override void Update()
        {
        }


    }

    public class CharacterTakeDamageStateBuilder : StateBuilder<CharacterTakeDamageState>
    {
        public CharacterTakeDamageStateBuilder(CharacterTakeDamageState instance) : base(instance)
        {
        }
    }
}