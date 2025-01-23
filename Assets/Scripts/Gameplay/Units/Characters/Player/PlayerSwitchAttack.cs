using System;
using Gameplay.Damage;
using Gameplay.Weapon;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerSwitchAttack : CharacterSwitchAttack
    {
        [Inject] private DiContainer diContainer;
        
        private PlayerAnimation playerAnimation;
        private PlayerWeaponAttackState playerWeaponAttackState;
        private PlayerDefaultAttackState playerDefaultAttackState;
        private SO_PlayerAttack so_PlayerAttack;
        private Weapon currentWeapon;
        
        public Transform WeaponParent { get; set; }


        public override int TotalDamage()
        {
            if (currentWeapon == null && playerDefaultAttackState != null)
            {
                return playerDefaultAttackState.Damageable.CurrentDamage + 
                       playerDefaultAttackState.Damageable.AdditionalDamage;
            }
            else if (currentWeapon != null && playerWeaponAttackState != null)
            {
                return playerWeaponAttackState.CurrentWeapon.Damageable.CurrentDamage +
                       playerWeaponAttackState.CurrentWeapon.Damageable.AdditionalDamage;
            }
            return 0;
        }

        public override int TotalAttackSpeed()
        {
            if (currentWeapon == null && playerDefaultAttackState != null)
            {
                return playerDefaultAttackState.AttackSpeed;
            }
            else if (currentWeapon != null && playerWeaponAttackState != null)
            {
                return playerWeaponAttackState.AttackSpeed;
            }
            return 0;
        }

        public override float TotalAttackRange()
        {
            if (currentWeapon == null && playerDefaultAttackState != null)
            {
                return playerDefaultAttackState.Range;
            }
            else if (currentWeapon != null && playerWeaponAttackState != null)
            {
                return playerWeaponAttackState.Range;
            }
            return 0;
        }


        private PlayerDefaultAttackState CreateDefaultAttackState()
        {
            return (PlayerDefaultAttackState)new PlayerDefaultAttackStateBuilder()
                .SetAttackClips(so_PlayerAttack.DefaultAttackClips)
                .SetCooldownClip(so_PlayerAttack.DefaultCooldownClip)
                .SetGameObject(GameObject)
                .SetCenter(Center)
                .SetCharacterAnimation(CharacterAnimation)
                .SetEnemyLayer(EnemyLayer)
                .SetAttackSpeed(so_PlayerAttack.AttackSpeed)
                .SetDamageable(base.damageable)
                .SetStateMachine(StateMachine)
                .Build();
        }
        
        private PlayerWeaponAttackState CreateWeaponState()
        {
            return (PlayerWeaponAttackState)new PlayerWeaponAttackStateStateBuilder()
                .SetSwordAttackClip(so_PlayerAttack.SwordAttackClip)
                .SetSwordCooldownClip(so_PlayerAttack.SwordCooldownClip)
                .SetBowAttackClip(so_PlayerAttack.BowAttackClip)
                .SetBowCooldownClip(so_PlayerAttack.BowCooldownClip)
                .SetWeaponParent(WeaponParent)
                .SetEnemyLayer(EnemyLayer)
                .SetCenter(Center)
                .SetCharacterEndurance(CharacterEndurance)
                .SetBaseReductionEndurance(so_PlayerAttack.BaseReductionEndurance)
                .SetCharacterSwitchMove(CharacterSwitchMove)
                .SetGameObject(this.GameObject)
                .SetCharacterAnimation(playerAnimation)
                .SetAttackSpeed(so_PlayerAttack.AttackSpeed)
                .SetDamageable(base.damageable)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        public override bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange<IPlayerAttackable>(Center.position, RangeAttack, EnemyLayer, ref findUnitColliders);
        }

        public override void Initialize()
        {
            base.Initialize();
            playerAnimation = (PlayerAnimation)CharacterAnimation;
            so_PlayerAttack = (SO_PlayerAttack)so_CharacterAttack;
            damageable = new NormalDamage(so_PlayerAttack.Damage, this.GameObject);
            diContainer.Inject(damageable);
        }

        private void InitializeWeaponAttackState()
        {
            if(!this.StateMachine.IsStateNotNull(typeof(PlayerWeaponAttackState)))
            {
                playerWeaponAttackState = CreateWeaponState();
                diContainer.Inject(playerWeaponAttackState);
                playerWeaponAttackState.Initialize();
                this.StateMachine.AddStates(playerWeaponAttackState);
            }
        }

        private void InitializeDefaultAttackState()
        {
            if(!this.StateMachine.IsStateNotNull(typeof(PlayerDefaultAttackState)))
            {
                playerDefaultAttackState = CreateDefaultAttackState();
                diContainer.Inject(playerDefaultAttackState);
                playerDefaultAttackState.Initialize();
                this.StateMachine.AddStates(playerDefaultAttackState);
            }
        }

        private void SetRangeAttack(float range)
        {
            RangeAttack = range;
            RangeAttackSqr = RangeAttack * RangeAttack;
        }

        public override void SetState()
        {
            base.SetState();
            if (currentWeapon != null)
            {
                InitializeWeaponAttackState();

                playerWeaponAttackState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerWeaponAttackState.GetType()))
                {
                    SetRangeAttack(currentWeapon.Range);
                    this.StateMachine.SetStates(desiredStates: playerWeaponAttackState.GetType());
                }
            }
            else
            {
                InitializeDefaultAttackState();

                playerDefaultAttackState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerDefaultAttackState.GetType()))
                {
                    SetRangeAttack(so_PlayerAttack.Range);
                    this.StateMachine.SetStates(desiredStates: playerDefaultAttackState.GetType());
                }
            }
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            if (currentWeapon != null)
            {
                InitializeWeaponAttackState();

                playerWeaponAttackState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerWeaponAttackState.GetType()))
                {
                    SetRangeAttack(currentWeapon.Range);
                    this.StateMachine.ExitCategory(category, playerWeaponAttackState.GetType());
                }
            }
            else
            {
                InitializeDefaultAttackState();

                playerDefaultAttackState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerDefaultAttackState.GetType()))
                {
                    SetRangeAttack(so_PlayerAttack.Range);
                    this.StateMachine.ExitCategory(category, playerDefaultAttackState.GetType());
                }
            }
        }

        public override void ExitOtherStates()
        {
            base.ExitOtherStates();
            if (currentWeapon != null)
            {
                InitializeWeaponAttackState();

                playerWeaponAttackState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerWeaponAttackState.GetType()))
                {
                    SetRangeAttack(currentWeapon.Range);
                    this.StateMachine.ExitOtherStates(playerWeaponAttackState.GetType());
                }
            }
            else
            {
                InitializeDefaultAttackState();

                playerDefaultAttackState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerDefaultAttackState.GetType()))
                {
                    SetRangeAttack(so_PlayerAttack.Range);
                    this.StateMachine.ExitOtherStates(playerDefaultAttackState.GetType());
                }
            }
        }


        public void SetWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            RangeAttack = currentWeapon.Range;
            RangeAttackSqr = RangeAttack * RangeAttack;
            
            if(!this.StateMachine.IsStateNotNull(typeof(PlayerWeaponAttackState)))
            {
                playerWeaponAttackState = CreateWeaponState();
                playerWeaponAttackState.Initialize();
                this.StateMachine.AddStates(playerWeaponAttackState);
            }

            playerWeaponAttackState.SetWeapon(currentWeapon);
        }
    }

    public class PlayerSwitchAttackBuilder : CharacterSwitchAttackBuilder
    {
        public PlayerSwitchAttackBuilder() : base(new PlayerSwitchAttack())
        {
        }

        public PlayerSwitchAttackBuilder SetWeaponParent(Transform weaponParent)
        {
            if (state is PlayerSwitchAttack playerSwitchAttackState)
                playerSwitchAttackState.WeaponParent = weaponParent;

            return this;
        }
    }
}