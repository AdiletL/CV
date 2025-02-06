using System;
using Gameplay.Damage;
using Gameplay.Effect;
using Gameplay.Factory.Character.Player;
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
    [RequireComponent(typeof(EffectHandler))]
    [RequireComponent(typeof(SkillHandler))]
    
    public class PlayerController : CharacterMainController, IItemInteractable, ITrapInteractable
    {
        public Action<GameObject> TriggerEnter;
        public Action<GameObject> TriggerExit;
        
        [Space]
        [SerializeField] private SO_PlayerMove so_PlayerMove;
        [SerializeField] private SO_PlayerAttack so_PlayerAttack;
        [SerializeField] private SO_PlayerControlDesktop so_PlayerControlDesktop;
        
        [Space]
        [SerializeField] private SO_Sword so_Sword;
        [SerializeField] private SO_Bow so_Bow;
        [SerializeField] private Transform weaponParent;

        [Space]
        [SerializeField] private GameObject rangeVisual;
        
        [Space]
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;

        private PlayerStateFactory playerStateFactory;
        private PlayerSwitchStateFactory playerSwitchStateFactory;
        
        private IControl playerControlDesktop;
        private CharacterSwitchMoveState characterSwitchMoveState;
        private CharacterSwitchAttackState characterSwitchAttackState;
        private CharacterEndurance characterEndurance;
        private CharacterExperience characterExperience;
        private CharacterAnimation characterAnimation;
        private UnitTransformSync unitTransformSync;
        
        private PlayerKinematicControl playerKinematicControl;
        
        protected override UnitInformation CreateUnitInformation()
        {
            return new PlayerInformation(this);
        }

        public override int TotalDamage() => characterSwitchAttackState.TotalDamage();
        public override int TotalAttackSpeed() => characterSwitchAttackState.TotalAttackSpeed();
        public override float TotalAttackRange() => characterSwitchAttackState.TotalAttackRange();
        

        private PlayerControlDesktop CreatePlayerControlDesktop()
        {
            return (PlayerControlDesktop)new PlayerControlDesktopBuilder()
                .SetPlayerKinematicControl(playerKinematicControl)
                .SetCharacterSwitchAttack(characterSwitchAttackState)
                .SetCharacterSwitchMove(characterSwitchMoveState)
                .SetPhotonView(photonView)
                .SetPlayerController(this)
                .SetPlayerStateFactory(playerStateFactory)
                .SetPlayerControlDesktopConfig(so_PlayerControlDesktop)
                .SetEnemyLayer(so_PlayerAttack.EnemyLayer)
                .SetStateMachine(this.StateMachine)
                .SetGameObject(gameObject)
                .Build();
        }

        private PlayerStateFactory CreatePlayerStateFactory()
        {
            return (PlayerStateFactory)new PlayerStateFactoryBuilder()
                .SetKPlayerKinematicControl(playerKinematicControl)
                .SetCharacterEndurance(characterEndurance)
                .SetPhotonView(photonView)
                .SetPlayerAttackConfig(so_PlayerAttack)
                .SetPlayerMoveConfig(so_PlayerMove)
                .SetCharacterAnimation(characterAnimation)
                .SetWeaponParent(weaponParent)
                .SetStateMachine(StateMachine)
                .SetUnitCenter(unitCenter)
                .SetGameObject(gameObject)
                .Build();
        }

        private PlayerSwitchStateFactory CreatePlayerSwitchStateFactory()
        {
            return (PlayerSwitchStateFactory)new PlayerSwitchStateFactoryBuilder()
                .SetCharacterState(playerStateFactory)
                .SetCharacterAnimation(characterAnimation)
                .SetCharacterEndurance(characterEndurance)
                .SetPhotonView(photonView)
                .SetWeaponParent(weaponParent)
                .SetPlayerAttackConfig(so_PlayerAttack)
                .SetPlayerMoveConfig(so_PlayerMove)
                .SetStateMachine(StateMachine)
                .SetGameObject(gameObject)
                .SetUnitCenter(unitCenter)
                .Build();
        }

        public override void Initialize()
        {
            base.Initialize();
            
            if(photonView.IsMine)
                FindFirstObjectByType<CameraMove>().SetTarget(gameObject);

            characterExperience = GetComponentInUnit<CharacterExperience>();
            diContainer.Inject(characterExperience);
            characterExperience.Initialize();
            
            characterEndurance = GetComponentInUnit<CharacterEndurance>();
            diContainer.Inject(characterEndurance);
            characterEndurance.Initialize();
            
            //Test
            InitializeNormalResistance();
            //Test
            InitializeSword();

            InitializeAllAnimations();
            
            StateMachine.Initialize();
            StateMachine.SetStates(desiredStates: typeof(PlayerIdleState));
        }

        protected override void BeforeCreateStates()
        {
            base.BeforeCreateStates();

            playerKinematicControl = GetComponentInUnit<PlayerKinematicControl>();
            characterEndurance = GetComponentInUnit<PlayerEndurance>();
            
            characterAnimation = GetComponentInUnit<PlayerAnimation>();
            diContainer.Inject(characterAnimation);
            characterAnimation.Initialize();
            
            unitTransformSync = GetComponentInUnit<UnitTransformSync>();
            playerStateFactory = CreatePlayerStateFactory();
            diContainer.Inject(playerStateFactory);
            playerSwitchStateFactory = CreatePlayerSwitchStateFactory();
            diContainer.Inject(playerSwitchStateFactory);
        }

        protected override void CreateStates()
        {
            var idleState = playerStateFactory.CreateState(typeof(PlayerIdleState));
            diContainer.Inject(idleState);
            
            StateMachine.AddStates(idleState);
        }

        protected override void CreateSwitchState()
        {
            characterSwitchMoveState = playerSwitchStateFactory.CreateSwitchMoveState(typeof(PlayerSwitchMoveState));
            diContainer.Inject(characterSwitchMoveState);
            playerStateFactory.SetCharacterSwitchMove(characterSwitchMoveState);
            
            characterSwitchAttackState = playerSwitchStateFactory.CreateSwitchAttackState(typeof(PlayerSwitchAttackState));
            diContainer.Inject(characterSwitchAttackState);
            playerStateFactory.SetCharacterSwitchAttack(characterSwitchAttackState);
            
            characterSwitchMoveState.SetSwitchAttackState(characterSwitchAttackState);
            characterSwitchAttackState.SetSwitchMoveState(characterSwitchMoveState);
            
            characterSwitchAttackState.Initialize();
            characterSwitchMoveState.Initialize();
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

        protected override void UnInitializeMediatorRPC()
        {
            base.UnInitializeMediatorRPC();
            StateMachine.OnChangedState -= OnChangedState;
        }

        public override void Appear()
        {
            unitTransformSync.enabled = true;
        }

        private void InitializeSword()
        {
            if (!photonView.IsMine) return;
            //TEST
            if (characterSwitchAttackState.TryGetWeapon(typeof(Sword), out Sword component))
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
            if (!photonView.IsMine) return;
            
            if (characterSwitchAttackState.TryGetWeapon(typeof(Bow), out Bow component))
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

        protected override void InitializeAllAnimations()
        {
            characterAnimation.AddClips(so_PlayerMove.IdleClip);
            characterAnimation.AddClips(so_PlayerMove.RunClip);
            characterAnimation.AddClips(so_PlayerAttack.BowAttackClip);
            characterAnimation.AddClip(so_PlayerAttack.BowCooldownClip);
            characterAnimation.AddClips(so_PlayerAttack.DefaultAttackClips);
            characterAnimation.AddClip(so_PlayerAttack.DefaultCooldownClip);
            characterAnimation.AddClips(so_PlayerAttack.SwordAttackClip);
            characterAnimation.AddClip(so_PlayerAttack.SwordCooldownClip);
            characterAnimation.AddClip(so_PlayerMove.JumpInfo.Clip);
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
            characterSwitchAttackState.SetWeapon(weapon);
            rangeVisual.transform.localScale = Vector3.one * (weapon.Range * 2);
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
