using System;
using Gameplay.Damage;
using Gameplay.Weapon;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerSwitchAttackState : CharacterSwitchAttackState
    {
        private PlayerAnimation playerAnimation;
        private SO_PlayerAttack so_PlayerAttack;
        private Weapon currentWeapon;
        private float rangeAttack;
        private int amountAttack;
        
        protected Collider[] findUnitColliders = new Collider[1];
        
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
                .SetAmountAttack(amountAttack)
                .SetDamageable(Damageable)
                .SetStateMachine(StateMachine)
                .Build();
        }
        
        private PlayerWeaponState CreateWeaponState()
        {
            return (PlayerWeaponState)new PlayerWeaponBuilder()
                .SetConfig(so_PlayerAttack)
                .SetWeaponParent(WeaponParent)
                .SetEnemyLayer(EnemyLayer)
                .SetCenter(Center)
                .SetGameObject(this.GameObject)
                .SetCharacterAnimation(playerAnimation)
                .SetDamageable(Damageable)
                .SetStateMachine(this.StateMachine)
                .Build();
        }
        
        public override bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange(Center.position, rangeAttack, EnemyLayer, findUnitColliders);
        }
        
        public override void Initialize()
        {
            base.Initialize();
            playerAnimation = (PlayerAnimation)CharacterAnimation;
            so_PlayerAttack = (SO_PlayerAttack)so_CharacterAttack;
            Damageable = new NormalDamage(so_PlayerAttack.Damage, this.GameObject);
            amountAttack = so_PlayerAttack.AmountAttack;
        }

        protected override void DestermineState()
        {
            //TODO: Type attack state

            if (currentWeapon != null)
            {
                if(!this.StateMachine.IsStateNotNull(typeof(PlayerWeaponState)))
                {
                    var newState = CreateWeaponState();
                    
                    newState.Initialize();
                    this.StateMachine.AddStates(newState);
                }

                rangeAttack = currentWeapon.Range;
                this.StateMachine.SetStates(typeof(PlayerWeaponState));
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
            
            if(!this.StateMachine.IsStateNotNull(typeof(PlayerWeaponState)))
            {
                var newState = CreateWeaponState();
                
                newState.Initialize();
                this.StateMachine.AddStates(newState);
            }

            this.StateMachine.GetState<PlayerWeaponState>().SetWeapon(currentWeapon);
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