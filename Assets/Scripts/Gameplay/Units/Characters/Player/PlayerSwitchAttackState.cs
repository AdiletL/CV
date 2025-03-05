using System;
using System.Collections.Generic;
using Gameplay.Equipment.Weapon;
using Gameplay.Factory.Character;
using Gameplay.Weapon;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerSwitchAttackState : CharacterSwitchAttackState
    {
        [Inject] private DiContainer diContainer;
        
        private CharacterStateFactory characterStateFactory;
        private CharacterWeaponAttackState characterWeaponAttackState;
        private CharacterDefaultAttackState characterDefaultAttackState;
        private SO_PlayerAttack so_PlayerAttack;
        

        public void SetCharacterSwitchStateFactory(CharacterStateFactory characterStateFactory) => this.characterStateFactory = characterStateFactory;
        
        
        public override bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange<IPlayerAttackable>(center.position, RangeAttack, enemyLayer, ref findUnitColliders);
        }

        public override void Initialize()
        {
            base.Initialize();
            so_PlayerAttack = (SO_PlayerAttack)so_CharacterAttack;
            InitializeWeaponAttackState();
        }

        private void InitializeWeaponAttackState()
        {
            if(!this.stateMachine.IsStateNotNull(typeof(PlayerWeaponAttackState)))
            {
                characterWeaponAttackState = (PlayerWeaponAttackState)characterStateFactory.CreateState(typeof(PlayerWeaponAttackState));
                diContainer.Inject(characterWeaponAttackState);
                characterWeaponAttackState.Initialize();
                this.stateMachine.AddStates(characterWeaponAttackState);
            }
        }

        private void InitializeDefaultAttackState()
        {
            if(!this.stateMachine.IsStateNotNull(typeof(PlayerDefaultAttackState)))
            {
                characterDefaultAttackState = (PlayerDefaultAttackState)characterStateFactory.CreateState(typeof(PlayerDefaultAttackState));
                diContainer.Inject(characterDefaultAttackState);
                characterDefaultAttackState.Initialize();
                this.stateMachine.AddStates(characterDefaultAttackState);
            }
        }

        private void SetRangeAttack(float range)
        {
            this.RangeAttack = range;
            RangeAttackSqr = RangeAttack * RangeAttack;
        }

        public override void SetState()
        {
            base.SetState();
            if (characterWeaponAttackState.CurrentWeapon != null)
            {
                InitializeWeaponAttackState();

                characterWeaponAttackState.SetTarget(currentTarget);
                if (!this.stateMachine.IsActivateType(characterWeaponAttackState.GetType()))
                {
                    this.stateMachine.SetStates(desiredStates: characterWeaponAttackState.GetType());
                }
            }
            else
            {
                InitializeDefaultAttackState();

                characterDefaultAttackState.SetTarget(currentTarget);
                if (!this.stateMachine.IsActivateType(characterDefaultAttackState.GetType()))
                {
                    this.stateMachine.SetStates(desiredStates: characterDefaultAttackState.GetType());
                }
            }
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            if (characterWeaponAttackState.CurrentWeapon != null)
            {
                InitializeWeaponAttackState();

                characterWeaponAttackState.SetTarget(currentTarget);
                if (!this.stateMachine.IsActivateType(characterWeaponAttackState.GetType()))
                {
                    this.stateMachine.ExitCategory(category, characterWeaponAttackState.GetType());
                }
            }
            else
            {
                InitializeDefaultAttackState();

                characterDefaultAttackState.SetTarget(currentTarget);
                if (!this.stateMachine.IsActivateType(characterDefaultAttackState.GetType()))
                {
                    SetRangeAttack(so_PlayerAttack.Range);
                    this.stateMachine.ExitCategory(category, characterDefaultAttackState.GetType());
                }
            }
        }

        public override void ExitOtherStates()
        {
            base.ExitOtherStates();
            if (characterWeaponAttackState.CurrentWeapon != null)
            {
                InitializeWeaponAttackState();

                characterWeaponAttackState.SetTarget(currentTarget);
                if (!this.stateMachine.IsActivateType(characterWeaponAttackState.GetType()))
                {
                    this.stateMachine.ExitOtherStates(characterWeaponAttackState.GetType());
                }
            }
            else
            {
                InitializeDefaultAttackState();

                characterDefaultAttackState.SetTarget(currentTarget);
                if (!this.stateMachine.IsActivateType(characterDefaultAttackState.GetType()))
                {
                    SetRangeAttack(so_PlayerAttack.Range);
                    this.stateMachine.ExitOtherStates(characterDefaultAttackState.GetType());
                }
            }
        }
    }

    public class PlayerSwitchAttackStateBuilder : CharacterSwitchAttackStateBuilder
    {
        public PlayerSwitchAttackStateBuilder() : base(new PlayerSwitchAttackState())
        {
        }

        public PlayerSwitchAttackStateBuilder SetCharacterStateFactory(CharacterStateFactory playerStateFactory)
        {
            if (switchState is PlayerSwitchAttackState playerSwitchAttackState)
                playerSwitchAttackState.SetCharacterSwitchStateFactory(playerStateFactory);

            return this;
        }
    }
}