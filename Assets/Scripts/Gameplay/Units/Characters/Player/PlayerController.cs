using System;
using System.Collections.Generic;
using Gameplay.Damage;
using Gameplay.Weapon;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using ScriptableObjects.Weapon;
using Unity.Collections;
using UnityEngine;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerController : CharacterMainController
    {
        [Inject] private DiContainer diContainer;
        public override UnitType UnitType { get; } = UnitType.player;
        
        [SerializeField] private SO_PlayerMove so_PlayerMove;
        [SerializeField] private SO_PlayerAttack so_PlayerAttack;
        [SerializeField] private SO_Sword so_Sword;
        [SerializeField] private SO_Bow so_Bow;
        [SerializeField] private Transform weaponParent;
        
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;

        private UnitControlDesktop unitControlDesktop;
        
        private GameObject finish;
        
        private CharacterController characterController;
        
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
                .SetCharacterController(characterController)
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

            unitControlDesktop = new PlayerControlDesktop(this);
            characterController = GetComponent<CharacterController>();
            
            CreateStates();
            
            StateMachine.Initialize();
            
            components.GetComponentFromArray<PlayerHealth>()?.Initialize();
            
            //TEST
            var swordDamageable = new NormalDamage(so_Sword.Damage, gameObject);
            var sword = (Sword)new SwordBuilder()
                .SetWeaponParent(weaponParent)
                .SetRange(so_Sword.Range)
                .SetAmountAttack(so_Sword.AmountAttackInSecond)
                .SetWeaponPrefab(so_Sword.WeaponPrefab)
                .SetDamageable(swordDamageable)
                .Build();
            sword.Initialize();
            SetWeapon(sword);

            /*var projectile = new NormalDamage(so_Bow.Damage, gameObject);
            var bow = (Bow)new BowBuilder()
                .SetDamageable(projectile)
                .SetRange(so_Bow.Range)
                .SetAmountAttack(so_Bow.AmountAttack)
                .SetWeaponParent(weaponParent)
                .SetWeaponPrefab(so_Bow.WeaponPrefab)
                .Build();
            diContainer.Inject(bow);
            bow.Initialize();
            SetWeapon(bow);*/
            
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

        
        public override void Appear()
        {
            
        }

        private void Update()
        {
            unitControlDesktop?.HandleInput();
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
            this.StateMachine?.GetState<PlayerIdleState>()?.SetTarget(finish);
        }

        private void OnChangedState(Machine.IState state)
        {
            currentStateCategory = state.Category;
            currentStateName = state.GetType().Name;
        }

        private void OnDestroy()
        {
            StateMachine.OnChangedState -= OnChangedState;
        }
    }
}
