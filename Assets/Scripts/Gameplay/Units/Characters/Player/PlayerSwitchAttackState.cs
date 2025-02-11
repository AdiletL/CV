using System;
using System.Collections.Generic;
using Gameplay.Factory.Character;
using Gameplay.Weapon;
using Machine;
using ScriptableObjects.Unit.Character.Player;
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
        
        private List<Weapon> currentWeapons = new();

        public void SetCharacterSwitchStateFactory(CharacterStateFactory characterStateFactory) => this.characterStateFactory = characterStateFactory;
        

        public override bool TryGetWeapon(Type weaponType)
        {
            foreach (var w in currentWeapons)
            {
                if (w.GetType() == weaponType)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool TryGetWeapon<T>(Type weaponType, out T weapon)
        {
            foreach (var w in currentWeapons)
            {
                if (w.GetType() == weaponType)
                {
                    weapon = (T)w;
                    return true;
                }
            }

            weapon = null;
            return false;
        }


        public override int TotalDamage()
        {
            if (currentWeapon == null && characterDefaultAttackState != null)
            {
                return characterDefaultAttackState.Damageable.CurrentDamage + 
                       characterDefaultAttackState.Damageable.AdditionalDamage;
            }
            else if (currentWeapon != null && characterWeaponAttackState != null)
            {
                return characterWeaponAttackState.CurrentWeapon.Damageable.CurrentDamage +
                       characterWeaponAttackState.CurrentWeapon.Damageable.AdditionalDamage;
            }
            return 0;
        }

        public override int TotalAttackSpeed()
        {
            if (currentWeapon == null && characterDefaultAttackState != null)
            {
                return characterDefaultAttackState.AttackSpeed;
            }
            else if (currentWeapon != null && characterWeaponAttackState != null)
            {
                return characterWeaponAttackState.AttackSpeed;
            }
            return 0;
        }

        public override float TotalAttackRange()
        {
            if (currentWeapon == null && characterDefaultAttackState != null)
            {
                return characterDefaultAttackState.Range;
            }
            else if (currentWeapon != null && characterWeaponAttackState != null)
            {
                return characterWeaponAttackState.Range;
            }
            return 0;
        }
        

        public override bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange<IPlayerAttackable>(center.position, RangeAttack, enemyLayer, ref findUnitColliders);
        }

        public override void Initialize()
        {
            base.Initialize();
            so_PlayerAttack = (SO_PlayerAttack)so_CharacterAttack;
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
            if (currentWeapon != null)
            {
                InitializeWeaponAttackState();

                characterWeaponAttackState.SetTarget(currentTarget);
                if (!this.stateMachine.IsActivateType(characterWeaponAttackState.GetType()))
                {
                    SetRangeAttack(currentWeapon.Range);
                    this.stateMachine.SetStates(desiredStates: characterWeaponAttackState.GetType());
                }
            }
            else
            {
                InitializeDefaultAttackState();

                characterDefaultAttackState.SetTarget(currentTarget);
                if (!this.stateMachine.IsActivateType(characterDefaultAttackState.GetType()))
                {
                    SetRangeAttack(so_PlayerAttack.Range);
                    this.stateMachine.SetStates(desiredStates: characterDefaultAttackState.GetType());
                }
            }
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            if (currentWeapon != null)
            {
                InitializeWeaponAttackState();

                characterWeaponAttackState.SetTarget(currentTarget);
                if (!this.stateMachine.IsActivateType(characterWeaponAttackState.GetType()))
                {
                    SetRangeAttack(currentWeapon.Range);
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
            if (currentWeapon != null)
            {
                InitializeWeaponAttackState();

                characterWeaponAttackState.SetTarget(currentTarget);
                if (!this.stateMachine.IsActivateType(characterWeaponAttackState.GetType()))
                {
                    SetRangeAttack(currentWeapon.Range);
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


        public override void SetWeapon(Weapon weapon)
        {
            if (!TryGetWeapon(weapon.GetType()))
                currentWeapons.Add(weapon);
                
            currentWeapon = weapon;
            SetRangeAttack(currentWeapon.Range);

            InitializeWeaponAttackState();

            characterWeaponAttackState.SetWeapon(currentWeapon);
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