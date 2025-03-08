using System;
using Gameplay.Effect;
using Gameplay.Factory.Character.Player;
using Gameplay.Resistance;
using Gameplay.Ability;
using Gameplay.Factory;
using ScriptableObjects.Unit.Character.Player;
using ScriptableObjects.Unit.Item;
using Unity.Collections;
using UnityEngine;
using ValueType = Calculate.ValueType;

namespace Gameplay.Unit.Character.Player
{
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerEndurance))]
    [RequireComponent(typeof(PlayerAnimation))]
    [RequireComponent(typeof(PlayerExperience))]
    [RequireComponent(typeof(EffectHandler))]
    [RequireComponent(typeof(AbilityHandler))]
    
    public class PlayerController : CharacterMainController, IItemInteractable, ITrapInteractable
    {
        public Action<GameObject> TriggerEnter;
        public Action<GameObject> TriggerExit;
        
        [Space]
        [SerializeField] private SO_PlayerMove so_PlayerMove;
        [SerializeField] private SO_PlayerAttack so_PlayerAttack;
        [SerializeField] private SO_PlayerControlDesktop so_PlayerControlDesktop;
        [SerializeField] private SO_PlayerAbilities so_PlayerAbilities;
        [SerializeField] private SO_PlayerSpecialAction so_PlayerSpecialAction;
        
        [Space]
        [SerializeField] private SO_NormalSword so_NormalSword;
        [SerializeField] private Transform weaponParent;
        
        [Space]
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;

        private PlayerStateFactory playerStateFactory;
        private PlayerItemInventory playerItemInventory;
        private PlayerAbilityInventory playerAbilityInventory;
        private PlayerKinematicControl playerKinematicControl;
        private PlayerControlDesktop playerControlDesktop;
        private PlayerStatsController playerStatsController;
        
        private CharacterEndurance characterEndurance;
        private CharacterExperience characterExperience;
        private CharacterAnimation characterAnimation;
        private UnitTransformSync unitTransformSync;

        public PlayerBlockInput PlayerBlockInput { get; private set; }
        public Camera BaseCamera { get; private set; }
        
        protected override UnitInformation CreateUnitInformation()
        {
            return new PlayerInformation(this);
        }
        
        private PlayerControlDesktop CreatePlayerControlDesktop()
        {
            return (PlayerControlDesktop)new PlayerControlDesktopBuilder()
                .SetPlayerAttackConfig(so_PlayerAttack)
                .SetPlayerMoveConfig(so_PlayerMove)
                .SetPlayerSpecialActionConfig(so_PlayerSpecialAction)
                .SetPlayerKinematicControl(playerKinematicControl)
                .SetConfig(so_PlayerControlDesktop)
                .SetPhotonView(photonView)
                .SetPlayerController(this)
                .SetPlayerStateFactory(playerStateFactory)
                .SetStateMachine(this.StateMachine)
                .SetGameObject(gameObject)
                .Build();
        }

        private PlayerStateFactory CreatePlayerStateFactory()
        {
            return (PlayerStateFactory)new PlayerStateFactoryBuilder()
                .SetUnitRenderer(unitRenderer)
                .SetPlayerSpecialActionConfig(so_PlayerSpecialAction)
                .SetPlayerKinematicControl(playerKinematicControl)
                .SetBaseCamera(BaseCamera)
                .SetCharacterEndurance(GetComponentInUnit<CharacterEndurance>())
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

        public override void Initialize()
        {
            //Test
            InitializeNormalResistance();
            base.Initialize();
            
            //Test
            InitializeSword();

            StateMachine.SetStates(desiredStates: typeof(PlayerIdleState));
        }

        protected override void BeforeCreateStates()
        {
            base.BeforeCreateStates();

            if (photonView.IsMine)
            {
                var cameraController = FindFirstObjectByType<CameraController>();
                BaseCamera = cameraController.GetComponent<Camera>();
                cameraController.CurrentCinemachineCamera.Follow = transform;
            }

            PlayerBlockInput = new PlayerBlockInput();
            PlayerBlockInput.Initialize();
            
            playerKinematicControl = GetComponentInUnit<PlayerKinematicControl>();
            diContainer.Inject(playerKinematicControl);
            playerKinematicControl.Initialize();
            
            characterAnimation = GetComponentInUnit<PlayerAnimation>();
            diContainer.Inject(characterAnimation);
            characterAnimation.Initialize();
            
            unitTransformSync = GetComponentInUnit<UnitTransformSync>();
            diContainer.Inject(unitTransformSync);
            
            playerStateFactory = CreatePlayerStateFactory();
            diContainer.Inject(playerStateFactory);
        }

        protected override void CreateStates()
        {
            var idleState = playerStateFactory.CreateState(typeof(PlayerIdleState));
            diContainer.Inject(idleState);
            StateMachine.AddStates(idleState);
            
            var attackState = playerStateFactory.CreateState(typeof(PlayerAttackState));
            diContainer.Inject(attackState);
            StateMachine.AddStates(attackState);
            
            var moveState = playerStateFactory.CreateState(typeof(PlayerMoveState));
            diContainer.Inject(moveState);
            StateMachine.AddStates(moveState);
        }

        protected override void AfterCreateStates()
        {
            base.AfterCreateStates();
            
            playerControlDesktop = CreatePlayerControlDesktop();
            diContainer.Inject(playerControlDesktop);
            playerControlDesktop.Initialize();
            
            playerItemInventory = GetComponentInUnit<PlayerItemInventory>();
            diContainer.Inject(playerItemInventory);
            playerItemInventory.Initialize();
            
            playerAbilityInventory = GetComponentInUnit<PlayerAbilityInventory>();
            diContainer.Inject(playerAbilityInventory);
            playerAbilityInventory.Initialize();
            
            playerStatsController = GetComponentInUnit<PlayerStatsController>();
            playerStatsController.Initialize();
        }

        protected override void AfterInitializeMediator()
        {
            base.AfterInitializeMediator();
            
            characterExperience = GetComponentInUnit<CharacterExperience>();
            diContainer.Inject(characterExperience);
            characterExperience.Initialize();
            
            characterEndurance = GetComponentInUnit<CharacterEndurance>();
            diContainer.Inject(characterEndurance);
            characterEndurance.Initialize();
        }

        protected override void SubscribeEvent()
        {
            base.SubscribeEvent();
            StateMachine.OnChangedState += OnChangedState;
            GetComponentInUnit<CharacterEndurance>().OnChangedEndurance += GetComponentInUnit<CharacterUI>().OnChangedEndurance;
            GetComponentInUnit<CharacterHealth>().OnChangedHealth += GetComponentInUnit<CharacterUI>().OnChangedHealth;
            GetComponentInUnit<CharacterHealth>().OnZeroHealth += OnZeroHealth;
        }

        protected override void UnSubscribeEvent()
        {
            base.UnSubscribeEvent();
            StateMachine.OnChangedState -= OnChangedState;
            GetComponentInUnit<CharacterEndurance>().OnChangedEndurance -= GetComponentInUnit<CharacterUI>().OnChangedEndurance;
            GetComponentInUnit<CharacterHealth>().OnChangedHealth -= GetComponentInUnit<CharacterUI>().OnChangedHealth;
            GetComponentInUnit<CharacterHealth>().OnZeroHealth -= OnZeroHealth;
        }

        public override void Appear()
        {
            unitTransformSync.enabled = true;
        }

        public override void Disappear()
        {
            throw new NotImplementedException();
        }

        //Test
        private void InitializeSword()
        {
            if (!photonView.IsMine) return;
            var inventoryItemFactory = new ItemInventoryFactoryBuilder()
                .Build();
            diContainer.Inject(inventoryItemFactory);
            inventoryItemFactory.Initialize();
            inventoryItemFactory.SetOwner(gameObject);
            var item = inventoryItemFactory.CreateItem(so_NormalSword);
            diContainer.Inject(item);
            item.SetAmountItem(1);
            item.SetStats(so_NormalSword.UnitStatsConfigs);
            item.Initialize();
            playerItemInventory.AddItem(item, so_NormalSword.Icon);
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
            characterAnimation.AddClip(so_PlayerMove.JumpConfig.Clip);
            characterAnimation.AddClip(so_PlayerSpecialAction.AbilityConfigData.BlockPhysicalDamageConfig.BlockClip);
        }
        
        private void Update()
        {
            if(!photonView.IsMine) return;
            
            playerControlDesktop?.HandleHotkey();
            playerControlDesktop?.HandleInput();
            StateMachine?.Update();
        }

        private void LateUpdate()
        {
            if(!photonView.IsMine) return;
            StateMachine?.LateUpdate();
        }
        
        public override void PutOnEquipment(Equipment.Equipment equipment)
        {
            base.PutOnEquipment(equipment);
            if (equipment is Equipment.Weapon.Weapon weapon)
            {
                StateMachine.GetState<PlayerAttackState>()?.SetWeapon(weapon);
            }
        }

        public override void TakeOffEquipment(Equipment.Equipment equipment)
        {
            base.TakeOffEquipment(equipment);
            if (equipment is Equipment.Weapon.Weapon weapon)
            {
                StateMachine.GetState<PlayerAttackState>()?.RemoveWeapon();
            }
        }

        private void OnChangedState(IState state)
        {
            currentStateCategory = state.Category;
            currentStateName = state.GetType().Name;
        }

        private void OnZeroHealth()
        {
            gameObject.SetActive(false);
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
