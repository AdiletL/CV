using System;
using System.Collections.Generic;
using Gameplay.Damage;
using Gameplay.Weapon;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using ScriptableObjects.Weapon;
using Unity.Collections;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerController : CharacterMainController
    {
        public override UnitType UnitType { get; } = UnitType.player;
        
        [SerializeField] private SO_PlayerMove so_PlayerMove;
        [SerializeField] private SO_PlayerAttack so_PlayerAttack;
        [SerializeField] private SO_Sword so_Sword;
        [SerializeField] private Transform weaponParent;
        
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;

        private GameObject finish;
        
        private PlayerIdleState CreateIdleState(CharacterAnimation characterAnimation, Transform center)
        {
            return (PlayerIdleState)new PlayerIdleStateBuilder()
                .SetMoveConfig(so_PlayerMove)
                .SetFinishTargetToMove(finish)
                .SetIdleClips(so_PlayerMove.IdleClip)
                .SetCharacterAnimation(characterAnimation)
                .SetCenter(center)
                .SetGameObject(gameObject)
                .SetStateMachine(StateMachine)
                .Build();
        }

        private PlayerSwitchMoveState CreateSwitchMoveState(CharacterAnimation characterAnimation, Transform center)
        {
            return (PlayerSwitchMoveState)new PlayerMoveStateBuilder()
                .SetCenter(center)
                .SetCharacterAnimation(characterAnimation)
                .SetConfig(so_PlayerMove)
                .SetGameObject(gameObject)
                .SetStateMachine(StateMachine)
                .Build();
        }

        private PlayerSwitchAttackState CreateSwitchAttackState(CharacterAnimation characterAnimation, Transform center)
        {
            return (PlayerSwitchAttackState)new PlayerSwitchAttackStateBuilder()
                .SetWeaponParent(weaponParent)
                .SetCenter(center)
                .SetEnemyLayer(Layers.CREEP_LAYER)
                .SetGameObject(gameObject)
                .SetCharacterAnimation(characterAnimation)
                .SetConfig(so_PlayerAttack)
                .SetStateMachine(StateMachine)
                .Build();
        }


        public override void Initialize()
        {
            base.Initialize();

            CreateStates();
            
            StateMachine.Initialize();
            
            components.GetComponentFromArray<PlayerHealth>()?.Initialize();
            
            //TEST
            var swordDamageable = new NormalDamage(so_Sword.Damage, gameObject);
            var sword = (Sword)new SwordBuilder()
                .SetWeaponParent(weaponParent)
                .SetRange(so_Sword.Range)
                .SetAmountAttack(so_Sword.AmountAttack)
                .SetWeaponPrefab(so_Sword.WeaponPrefab)
                .SetDamageable(swordDamageable)
                .Build();
            sword.Initialize();
            SetWeapon(sword);
            
            StateMachine.OnChangedState += OnChangedState;
            StateMachine.SetStates(typeof(PlayerIdleState));
        }

        private void CreateStates()
        {
            StateMachine = new StateMachine();

            var characterAnimation = components.GetComponentFromArray<CharacterAnimation>();
            var center = components.GetComponentFromArray<UnitCenter>().Center;

            var idleState = CreateIdleState(characterAnimation, center);
            var moveState = CreateSwitchMoveState(characterAnimation, center);
            var attackState = CreateSwitchAttackState(characterAnimation, center);
            
            StateMachine.AddStates(idleState, attackState, moveState);
        }


        private void Update()
        {
            StateMachine?.Update();
        }

        private void LateUpdate()
        {
            StateMachine?.LateUpdate();
        }
        
        public void SetWeapon(Weapon weapon)
        {
            this.StateMachine.GetState<PlayerSwitchAttackState>().SetWeapon(weapon);
        }

        public void IncreaseWeaponStates()
        {
            
        }
        public void SetFinishTarget(GameObject target)
        { 
            finish = target;
            this.StateMachine?.GetState<PlayerIdleState>()?.SetFinishTarget(finish);
        }

        private void OnChangedState(StateCategory category, Machine.IState state)
        {
            currentStateCategory = category;
            currentStateName = state.GetType().Name;
        }

        private void OnDestroy()
        {
            StateMachine.OnChangedState -= OnChangedState;
        }
    }
}