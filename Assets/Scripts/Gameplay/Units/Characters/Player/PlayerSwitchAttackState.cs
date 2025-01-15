using System;
using Gameplay.Damage;
using Gameplay.Weapon;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerSwitchAttackState : CharacterSwitchAttackState
    {
        private DiContainer diContainer;
        private PlayerAnimation playerAnimation;
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

        protected override void DestermineState()
        {
            //TODO: Type attack state

            if (currentWeapon != null)
            {
                if(!this.StateMachine.IsStateNotNull(typeof(PlayerWeaponAttackState)))
                {
                    var newState = CreateWeaponState();
                    
                    newState.Initialize();
                    this.StateMachine.AddStates(newState);
                }

                rangeAttack = currentWeapon.Range;
                this.StateMachine.SetStates(typeof(PlayerWeaponAttackState));
            }
            else
            {
                if(!this.StateMachine.IsStateNotNull(typeof(PlayerDefaultAttackState)))
                {
                    var newState = CreateDefaultAttackState();
                    
                    newState.Initialize();
                    this.StateMachine.AddStates(newState);
                }

                rangeAttack = so_PlayerAttack.Range;
                this.StateMachine.SetStates(typeof(PlayerDefaultAttackState));
            }
        }
        

        public void SetWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            rangeAttack = currentWeapon.Range;
            
            if(!this.StateMachine.IsStateNotNull(typeof(PlayerWeaponAttackState)))
            {
                var newState = CreateWeaponState();
                
                newState.Initialize();
                this.StateMachine.AddStates(newState);
            }

            this.StateMachine.GetState<PlayerWeaponAttackState>().SetWeapon(currentWeapon);
        }
    }

    public class PlayerSwitchAttackStateBuilder : CharacterSwitchAttackStateBuilder
    {
        public PlayerSwitchAttackStateBuilder() : base(new PlayerSwitchAttackState())
        {
        }

        public PlayerSwitchAttackStateBuilder SetWeaponParent(Transform weaponParent)
        {
            if (state is PlayerSwitchAttackState playerSwitchAttackState)
                playerSwitchAttackState.WeaponParent = weaponParent;

            return this;
        }
    }
}