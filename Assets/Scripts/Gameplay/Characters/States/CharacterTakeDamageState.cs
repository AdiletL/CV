﻿using UnityEngine;

namespace Character
{
    public class CharacterTakeDamageState : State
    {
        public override void Initialize()
        {
            
        }

        public override void Enter()
        {
        }

        public override void Update()
        {
        }

        public override void Exit()
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