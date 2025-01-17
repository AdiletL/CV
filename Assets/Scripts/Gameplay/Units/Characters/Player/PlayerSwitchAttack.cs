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
        private DiContainer diContainer;
        private PlayerAnimation playerAnimation;
        private PlayerWeaponAttackState playerWeaponAttackState;
        private PlayerDefaultAttackState playerDefaultAttackState;
        private SO_PlayerAttack so_PlayerAttack;
        private Weapon currentWeapon;
        
        public Transform WeaponParent { get; set; }
        

        private PlayerDefaultAttackState CreateDefaultAttackState()
        {
            return (PlayerDefaultAttackState)new PlayerDefaultAttackStateBuilder()
                .SetAttackClips(so_PlayerAttack.DefaultAttackClips)
                .SetCooldownClip(so_PlayerAttack.DefaultCooldownClip)
                .SetGameObject(GameObject)
                .SetCenter(Center)
                .SetCharacterAnimation(CharacterAnimation)
                .SetEnemyLayer(EnemyLayer)
                .SetAmountAttackInSecond(so_PlayerAttack.AmountAttackInSecond)
                .SetDamageable(base.damageable)
                .SetStateMachine(StateMachine)
                .Build();
        }
        
        private PlayerWeaponAttackState CreateWeaponState()
        {
            return (PlayerWeaponAttackState)new PlayerWeaponAttackStateStateBuilder()
                .SetConfig(so_PlayerAttack)
                .SetWeaponParent(WeaponParent)
                .SetEnemyLayer(EnemyLayer)
                .SetCenter(Center)
                .SetCharacterEndurance(CharacterEndurance)
                .SetCharacterSwitchMove(CharacterSwitchMove)
                .SetGameObject(this.GameObject)
                .SetCharacterAnimation(playerAnimation)
                .SetAmountAttackInSecond(so_PlayerAttack.AmountAttackInSecond)
                .SetDamageable(base.damageable)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            playerAnimation = (PlayerAnimation)CharacterAnimation;
            so_PlayerAttack = (SO_PlayerAttack)so_CharacterAttack;
            damageable = new NormalDamage(so_PlayerAttack.Damage, this.GameObject);
            diContainer.Inject(damageable);
        }

        public override void SetState()
        {
            base.SetState();
            if (currentWeapon != null)
            {
                if(!this.StateMachine.IsStateNotNull(typeof(PlayerWeaponAttackState)))
                {
                    playerWeaponAttackState = CreateWeaponState();
                    diContainer.Inject(playerWeaponAttackState);
                    playerWeaponAttackState.Initialize();
                    this.StateMachine.AddStates(playerWeaponAttackState);
                }

                playerWeaponAttackState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerWeaponAttackState.GetType()))
                {
                    RangeAttack = currentWeapon.Range;
                    RangeAttackSqr = RangeAttack * RangeAttack;
                    Debug.Log(RangeAttackSqr);
                    this.StateMachine.SetStates(playerWeaponAttackState.GetType());
                }
            }
            else
            {
                if(!this.StateMachine.IsStateNotNull(typeof(PlayerDefaultAttackState)))
                {
                    playerDefaultAttackState = CreateDefaultAttackState();
                    diContainer.Inject(playerDefaultAttackState);
                    playerDefaultAttackState.Initialize();
                    this.StateMachine.AddStates(playerDefaultAttackState);
                }

                playerDefaultAttackState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerDefaultAttackState.GetType()))
                {
                    RangeAttack = so_PlayerAttack.Range;
                    RangeAttackSqr = RangeAttack * RangeAttack;
                    this.StateMachine.SetStates(playerDefaultAttackState.GetType());
                }
            }
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            if (currentWeapon != null)
            {
                if(!this.StateMachine.IsStateNotNull(typeof(PlayerWeaponAttackState)))
                {
                    playerWeaponAttackState = CreateWeaponState();
                    playerWeaponAttackState.Initialize();
                    this.StateMachine.AddStates(playerWeaponAttackState);
                }

                playerWeaponAttackState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerWeaponAttackState.GetType()))
                {
                    RangeAttack = currentWeapon.Range;
                    RangeAttackSqr = RangeAttack * RangeAttack;
                    Debug.Log(RangeAttackSqr);
                    this.StateMachine.ExitCategory(category, playerWeaponAttackState.GetType());
                }
            }
            else
            {
                if(!this.StateMachine.IsStateNotNull(typeof(PlayerDefaultAttackState)))
                {
                    playerDefaultAttackState = CreateDefaultAttackState();
                    playerDefaultAttackState.Initialize();
                    this.StateMachine.AddStates(playerDefaultAttackState);
                }

                playerDefaultAttackState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerDefaultAttackState.GetType()))
                {
                    RangeAttack = so_PlayerAttack.Range;
                    RangeAttackSqr = RangeAttack * RangeAttack;
                    this.StateMachine.ExitCategory(category, playerDefaultAttackState.GetType());
                }
            }
        }

        public override void ExitOtherStates()
        {
            base.ExitOtherStates();
            if (currentWeapon != null)
            {
                if(!this.StateMachine.IsStateNotNull(typeof(PlayerWeaponAttackState)))
                {
                    playerWeaponAttackState = CreateWeaponState();
                    playerWeaponAttackState.Initialize();
                    this.StateMachine.AddStates(playerWeaponAttackState);
                }

                playerWeaponAttackState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerWeaponAttackState.GetType()))
                {
                    RangeAttack = currentWeapon.Range;
                    RangeAttackSqr = RangeAttack * RangeAttack;
                    this.StateMachine.ExitOtherStates(playerWeaponAttackState.GetType());
                }
            }
            else
            {
                if(!this.StateMachine.IsStateNotNull(typeof(PlayerDefaultAttackState)))
                {
                    playerDefaultAttackState = CreateDefaultAttackState();
                    playerDefaultAttackState.Initialize();
                    this.StateMachine.AddStates(playerDefaultAttackState);
                }

                playerDefaultAttackState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerDefaultAttackState.GetType()))
                {
                    RangeAttack = so_PlayerAttack.Range;
                    RangeAttackSqr = RangeAttack * RangeAttack;
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