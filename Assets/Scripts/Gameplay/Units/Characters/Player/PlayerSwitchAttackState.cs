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
        
        public Transform WeaponParent { get; set; }
        
        protected bool isCheckDistanceToTarget(Vector3 targetPosition)
        {
            float distanceToTarget = Vector3.Distance(this.GameObject.transform.position, targetPosition);
            if (distanceToTarget > rangeAttack)
                return false;
            else
                return true;
        }
        
        public bool IsCheckTarget()
        {
            Collider[] hits = Physics.OverlapSphere(Center.position, rangeAttack, EnemyLayer);

            if (hits.Length == 0) return false;

            if (!hits[0].TryGetComponent(out IHealth health) || !health.IsLive) return false;
            if (!isCheckDistanceToTarget(hits[0].transform.position)) return false;
            if (hits[0].TryGetComponent(out UnitCenter unitCenter))
            {
                var direcitonToTarget = (unitCenter.Center.position - Center.position).normalized;
                if (Physics.Raycast(Center.position, direcitonToTarget, out var hit, rangeAttack)
                    && hit.transform.gameObject == hits[0].gameObject)
                {
                    return true;
                }
            }

            return false;
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
                .SetDamageble(Damageable)
                .SetStateMachine(this.StateMachine)
                .Build();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            playerAnimation = (PlayerAnimation)CharacterAnimation;
            so_PlayerAttack = (SO_PlayerAttack)SO_CharacterAttack;
            Damageable = new NormalDamage(so_PlayerAttack.Damage, this.GameObject);
        }

        protected override void DestermineState()
        {
            //TODO: Type attack state
            
            if(!this.StateMachine.IsStateNotNull(typeof(PlayerWeaponState)))
            {
                var newState = CreateWeaponState();
                
                newState.Initialize();
                this.StateMachine.AddStates(newState);
            }
            
            if(currentWeapon != null)
                this.StateMachine.SetStates(typeof(PlayerWeaponState));
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