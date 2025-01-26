using System;
using Gameplay.Damage;
using Gameplay.Effect;
using Gameplay.Resistance;
using Gameplay.Skill;
using Gameplay.Weapon;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using ScriptableObjects.Weapon;
using Unity.Collections;
using UnityEngine;
using ValueType = Calculate.ValueType;

namespace Unit.Character.Player
{
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerEndurance))]
    [RequireComponent(typeof(PlayerAnimation))]
    [RequireComponent(typeof(PlayerExperience))]
    [RequireComponent(typeof(PlayerGravity))]
    [RequireComponent(typeof(EffectHandler))]
    [RequireComponent(typeof(SkillHandler))]
    
    public class PlayerController : CharacterMainController, IItemInteractable, ITrapInteractable
    {
        public Action<GameObject> TriggerEnter;
        public Action<GameObject> TriggerExit;
        
        [SerializeField] private SO_PlayerMove so_PlayerMove;
        [SerializeField] private SO_PlayerAttack so_PlayerAttack;
        [SerializeField] private SO_PlayerControlDesktop so_PlayerControlDesktop;
        
        [Space]
        [SerializeField] private SO_Sword so_Sword;
        [SerializeField] private SO_Bow so_Bow;
        [SerializeField] private Transform weaponParent;
        
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;

        private IControl playerControlDesktop;
        private PlayerSwitchMove playerSwitchMove;
        private PlayerSwitchAttack playerSwitchAttack;
        private PlayerEndurance playerEndurance;
        
        private CharacterController characterController;
        
        protected override UnitInformation CreateUnitInformation()
        {
            return new PlayerInformation(this);
        }

        public override int TotalDamage() => playerSwitchAttack.TotalDamage();
        public override int TotalAttackSpeed() => playerSwitchAttack.TotalAttackSpeed();
        public override float TotalAttackRange() => playerSwitchAttack.TotalAttackRange();

        private PlayerIdleState CreateIdleState(CharacterAnimation characterAnimation, Transform center)
        {
            return (PlayerIdleState)new PlayerIdleStateBuilder()
                .SetIdleClips(so_PlayerMove.IdleClip)
                .SetCharacterSwitchMove(playerSwitchMove)
                .SetCharacterController(characterController)
                .SetCharacterAnimation(characterAnimation)
                .SetCenter(center)
                .SetGameObject(gameObject)
                .SetStateMachine(StateMachine)
                .Build();
        }

        private PlayerSwitchMove CreateSwitchMoveState(CharacterAnimation characterAnimation, Transform center)
        {
            return (PlayerSwitchMove)new PlayerSwitchMoveBuilder()
                .SetCharacterController(characterController)
                .SetCenter(center)
                .SetPlayerEndurance(playerEndurance)
                .SetCharacterAnimation(characterAnimation)
                .SetConfig(so_PlayerMove)
                .SetGameObject(gameObject)
                .SetRotationSpeed(so_PlayerMove.RotateSpeed)
                .SetStateMachine(StateMachine)
                .Build();
        }

        private PlayerSwitchAttack CreateSwitchAttackState(CharacterAnimation characterAnimation, Transform center)
        {
            return (PlayerSwitchAttack)new PlayerSwitchAttackBuilder()
                .SetWeaponParent(weaponParent)
                .SetCenter(center)
                .SetCharacterEndurance(playerEndurance)
                .SetEnemyLayer(so_PlayerAttack.EnemyLayer)
                .SetGameObject(gameObject)
                .SetCharacterAnimation(characterAnimation)
                .SetStateMachine(StateMachine)
                .SetConfig(so_PlayerAttack)
                .Build();
        }

        private PlayerControlDesktop CreatePlayerControlDesktop()
        {
            return (PlayerControlDesktop)new PlayerControlDesktopBuilder()
                .SetCharacterController(characterController)
                .SetGameObject(gameObject)
                .SetPlayerSwitchAttack(playerSwitchAttack)
                .SetPlayerSwitchMove(playerSwitchMove)
                .SetEndurance(playerEndurance)
                .SetPhotonView(photonView)
                .SetPlayerController(this)
                .SetPlayerAnimation(GetComponent<PlayerAnimation>())
                .SetPlayerMoveConfig(so_PlayerMove)
                .SetPlayerControlDesktopConfig(so_PlayerControlDesktop)
                .SetEnemyLayer(so_PlayerAttack.EnemyLayer)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        
        public override void Initialize()
        {
            characterController = GetComponent<CharacterController>();
            playerEndurance = GetComponentInUnit<PlayerEndurance>();
            
            base.Initialize();
            
            StateMachine.Initialize();
            
            //Test
            InitializeNormalResistance();
            //Test
            InitializeSword();
                        
            StateMachine.SetStates(desiredStates: typeof(PlayerIdleState));
        }

        protected override void CreateStates()
        {
            var characterAnimation = GetComponentInUnit<CharacterAnimation>();
            var center = GetComponentInUnit<UnitCenter>().Center;

            var idleState = CreateIdleState(characterAnimation, center);
            diContainer.Inject(idleState);
            
            StateMachine.AddStates(idleState);
        }

        protected override void CreateSwitchState()
        {
            var characterAnimation = GetComponentInUnit<CharacterAnimation>();
            var center = GetComponentInUnit<UnitCenter>().Center;

            playerSwitchMove = CreateSwitchMoveState(characterAnimation, center);
            diContainer.Inject(playerSwitchMove);
            
            playerSwitchAttack = CreateSwitchAttackState(characterAnimation, center);
            diContainer.Inject(playerSwitchAttack);
            
            playerSwitchMove.SetSwitchAttack(playerSwitchAttack);
            playerSwitchAttack.SetSwitchMove(playerSwitchMove);
            
            playerSwitchAttack.Initialize();
            playerSwitchMove.Initialize();
        }

        protected override void BeforeInitializeMediator()
        {
            base.BeforeInitializeMediator();
            playerControlDesktop = CreatePlayerControlDesktop();
            diContainer.Inject(playerControlDesktop);
            playerControlDesktop.Initialize();
        }

        protected override void InitializeMediator()
        {
            base.InitializeMediator();
            StateMachine.OnChangedState += OnChangedState;
        }

        protected override void UnInitializeMediator()
        {
            base.UnInitializeMediator();
            StateMachine.OnChangedState -= OnChangedState;
        }

        public override void Appear()
        {

        }

        private void InitializeSword()
        {
            //TEST
            if (playerSwitchAttack.TryGetWeapon(typeof(Sword), out Sword component))
            {
                SetWeapon(component);
            }
            else
            {
                var swordDamageable = new NormalDamage(so_Sword.Damage, gameObject);
                diContainer.Inject(swordDamageable);
                var sword = (Sword)new SwordBuilder()
                    .SetIncreaseAttackSpeed(so_Sword.IncreaseAttackSpeed.ValueType, so_Sword.IncreaseAttackSpeed.Value)
                    .SetReductionEndurance(so_Sword.ReductionEndurance.ValueType, so_Sword.ReductionEndurance.Value)
                    .SetAngleToTarget(so_Sword.AngleToTarget)
                    .SetWeaponParent(weaponParent)
                    .SetGameObject(gameObject)
                    .SetRange(so_Sword.Range)
                    .SetWeaponPrefab(so_Sword.WeaponPrefab)
                    .SetDamageable(swordDamageable)
                    .Build();
                diContainer.Inject(sword);
                sword.Initialize();
                SetWeapon(sword);
            }
            isSword = true;
            Debug.Log("Sword");
        }

        private void InitializeBow()
        {
            if (playerSwitchAttack.TryGetWeapon(typeof(Bow), out Bow component))
            {
                SetWeapon(component);
            }
            else
            {
                var projectile = new NormalDamage(so_Bow.Damage, gameObject);
                diContainer.Inject(projectile);
                var bow = (Bow)new BowBuilder()
                    .SetDamageable(projectile)
                    .SetRange(so_Bow.Range)
                    .SetGameObject(gameObject)
                    .SetWeaponParent(weaponParent)
                    .SetWeaponPrefab(so_Bow.WeaponPrefab)
                    .SetAngleToTarget(so_Bow.AngleToTarget)
                    .SetReductionEndurance(so_Bow.ReductionEndurance.ValueType, so_Bow.ReductionEndurance.Value)
                    .SetIncreaseAttackSpeed(so_Bow.IncreaseAttackSpeed.ValueType, so_Bow.IncreaseAttackSpeed.Value)
                    .Build();
                diContainer.Inject(bow);
                bow.Initialize();
                SetWeapon(bow);
            }
            isSword = false;
            Debug.Log("Bow");
        }

        //Test
        private void InitializeNormalResistance()
        {
            var normalDamageResistance = new NormalDamageResistance(80, ValueType.Percent);
            GetComponentInUnit<ResistanceHandler>().AddResistance(normalDamageResistance);
        }
        
        //Test
        private bool isSword;
        private void Update()
        {
            if(!photonView.IsMine) return;
            //Test
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (isSword)
                {
                    InitializeBow();
                }
                else
                {
                    InitializeSword();
                }
            }

            playerControlDesktop?.HandleHotkey();
            playerControlDesktop?.HandleInput();
            StateMachine?.Update();
        }

        private void LateUpdate()
        {
            if(!photonView.IsMine) return;
            StateMachine?.LateUpdate();
        }
        
        public void SetWeapon(Weapon weapon)
        {
            playerSwitchAttack.SetWeapon(weapon);
        }

        private void OnChangedState(Machine.IState state)
        {
            currentStateCategory = state.Category;
            currentStateName = state.GetType().Name;
        }


        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter?.Invoke(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerExit?.Invoke(other.gameObject);
        }
    }
}
