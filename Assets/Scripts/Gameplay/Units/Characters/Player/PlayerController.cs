using System;
using Gameplay.Effect;
using Gameplay.Factory.Character.Player;
using Gameplay.Resistance;
using Gameplay.Ability;
using Gameplay.Factory;
using Gameplay.Unit.Item;
using ScriptableObjects.Unit.Character.Player;
using ScriptableObjects.Unit.Item;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay.Unit.Character.Player
{
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerEndurance))]
    [RequireComponent(typeof(PlayerAnimation))]
    [RequireComponent(typeof(PlayerExperience))]
    [RequireComponent(typeof(EffectHandler))]
    [RequireComponent(typeof(PlayerItemInventory))]
    [RequireComponent(typeof(PlayerAbilityInventory))]
    [RequireComponent(typeof(AbilityHandler))]
    [RequireComponent(typeof(ResistanceHandler))]
    [RequireComponent(typeof(ItemHandler))]
    [RequireComponent(typeof(PlayerKinematicControl))]
    [RequireComponent(typeof(PlayerEquipmentController))]
    [RequireComponent(typeof(PlayerDisableController))]
    [RequireComponent(typeof(CharacterPortrait))]
    public class PlayerController : CharacterMainController, IItemInteractable, ITrapInteractable, ICreepInteractable, IPlayerController
    {
        public Action<GameObject> TriggerEnter;
        public Action<GameObject> TriggerExit;
        
        [Inject] private InventoryItemFactory inventoryItemFactory;
        
        [Space]
        [SerializeField] private SO_PlayerMove so_PlayerMove;
        [SerializeField] private SO_PlayerAttack so_PlayerAttack;
        [SerializeField] private SO_PlayerControlDesktop so_PlayerControlDesktop;
        [SerializeField] private SO_PlayerAbilities so_PlayerAbilities;
        [SerializeField] private SO_PlayerSpecialAction so_PlayerSpecialAction;

        [FormerlySerializedAs("so_NormalSword")]
        [Space]
        [SerializeField] private SO_NormalSwordItem soNormalSwordItem;
        
        [Space]
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;

        private PlayerItemInventory playerItemInventory;
        private PlayerAbilityInventory playerAbilityInventory;
        private PlayerKinematicControl playerKinematicControl;
        private PlayerControlDesktop playerControlDesktop;
        private PlayerStatsController playerStatsController;
        private PlayerEquipmentController playerEquipmentController;
        private PlayerUI playerUI;
        private PlayerHealth playerHealth;
        private PlayerMana playerMana;
        private CharacterPortrait characterPortrait;
        
        private CharacterEndurance characterEndurance;
        private CharacterExperience characterExperience;
        private CharacterAnimation characterAnimation;
        private UnitTransformSync unitTransformSync;

        public PlayerStateFactory PlayerStateFactory { get; private set; }
        public PlayerBlockInput PlayerBlockInput { get; private set; }
        public Camera BaseCamera { get; private set; }
        
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
                .SetPlayerStateFactory(PlayerStateFactory)
                .SetStateMachine(this.StateMachine)
                .SetGameObject(gameObject)
                .Build();
        }

        private PlayerStateFactory CreatePlayerStateFactory()
        {
            return (PlayerStateFactory)new PlayerStateFactoryBuilder()
                .SetUnitRenderer(unitRenderer)
                .SetPlayerItemInventory(GetComponentInUnit<PlayerItemInventory>())
                .SetPlayerSpecialActionConfig(so_PlayerSpecialAction)
                .SetPlayerKinematicControl(playerKinematicControl)
                .SetBaseCamera(BaseCamera)
                .SetCharacterStatsController(GetComponentInUnit<CharacterStatsController>())
                .SetPhotonView(photonView)
                .SetPlayerAttackConfig(so_PlayerAttack)
                .SetPlayerMoveConfig(so_PlayerMove)
                .SetCharacterAnimation(characterAnimation)
                .SetStateMachine(StateMachine)
                .SetUnitCenter(unitCenter)
                .SetGameObject(gameObject)
                .Build();
        }

        public override void Initialize()
        {
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
            
            GetComponentInUnit<ResistanceHandler>().Initialize();
            
            playerKinematicControl = GetComponentInUnit<PlayerKinematicControl>();
            diContainer.Inject(playerKinematicControl);
            playerKinematicControl.Initialize();
            
            characterAnimation = GetComponentInUnit<PlayerAnimation>();
            diContainer.Inject(characterAnimation);
            characterAnimation.Initialize();
            
            unitTransformSync = GetComponentInUnit<UnitTransformSync>();
            diContainer.Inject(unitTransformSync);
            
            playerEquipmentController = GetComponentInUnit<PlayerEquipmentController>();
            diContainer.Inject(playerEquipmentController);
            playerEquipmentController.Initialize();
            
            PlayerStateFactory = CreatePlayerStateFactory();
            diContainer.Inject(PlayerStateFactory);
            
            playerUI = GetComponentInUnit<PlayerUI>();
            diContainer.Inject(playerUI);
            playerUI.Initialize();
        }

        protected override void CreateStates()
        {
            var idleState = PlayerStateFactory.CreateState(typeof(PlayerIdleState));
            diContainer.Inject(idleState);
            StateMachine.AddStates(idleState);
            
            var attackState = PlayerStateFactory.CreateState(typeof(PlayerAttackState));
            diContainer.Inject(attackState);
            StateMachine.AddStates(attackState);
            
            var moveState = PlayerStateFactory.CreateState(typeof(PlayerMoveState));
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
            
            var playerDisableController = GetComponentInUnit<PlayerDisableController>();
            diContainer.Inject(playerDisableController);
            playerDisableController.Initialize();
        }

        protected override void AfterInitializeMediator()
        {
            base.AfterInitializeMediator();
            
            playerHealth = GetComponentInUnit<PlayerHealth>();
            diContainer.Inject(playerHealth);
            playerHealth.Initialize();
            
            playerMana = GetComponentInUnit<PlayerMana>();
            diContainer.Inject(playerMana);
            playerMana.Initialize();
            
            characterExperience = GetComponentInUnit<CharacterExperience>();
            diContainer.Inject(characterExperience);
            characterExperience.Initialize();
            
            characterEndurance = GetComponentInUnit<CharacterEndurance>();
            diContainer.Inject(characterEndurance);
            characterEndurance.Initialize();
            
            characterPortrait = GetComponentInUnit<CharacterPortrait>();
            diContainer.Inject(characterPortrait);
            characterPortrait.Initialize(playerStatsController);
        }

        protected override void InitializeMediatorEvent()
        {
            base.InitializeMediatorEvent();
            StateMachine.OnChangedState += OnChangedState;
            GetComponentInUnit<CharacterEndurance>().EnduranceStat.OnChangedCurrentValue += OnChangedEndurance; 
            GetComponentInUnit<CharacterEndurance>().EnduranceStat.OnChangedMaximumValue += OnChangedEndurance; 
            GetComponentInUnit<CharacterHealth>().HealthStat.OnChangedCurrentValue += OnChangedHealth;
            GetComponentInUnit<CharacterHealth>().HealthStat.OnChangedMaximumValue += OnChangedHealth;
            GetComponentInUnit<CharacterMana>().ManaStat.OnChangedCurrentValue += OnChangedMana;
            GetComponentInUnit<CharacterMana>().ManaStat.OnChangedMaximumValue += OnChangedMana;
            GetComponentInUnit<CharacterHealth>().OnZeroHealth += OnZeroHealth;
        }

        protected override void DeInitializeMediatorEvent()
        {
            base.DeInitializeMediatorEvent();
            StateMachine.OnChangedState -= OnChangedState;
            GetComponentInUnit<CharacterEndurance>().EnduranceStat.OnChangedCurrentValue -= OnChangedEndurance; 
            GetComponentInUnit<CharacterEndurance>().EnduranceStat.OnChangedMaximumValue -= OnChangedEndurance; 
            GetComponentInUnit<CharacterHealth>().HealthStat.OnChangedCurrentValue -= OnChangedHealth;
            GetComponentInUnit<CharacterHealth>().HealthStat.OnChangedMaximumValue -= OnChangedHealth;
            GetComponentInUnit<CharacterMana>().ManaStat.OnChangedCurrentValue -= OnChangedMana;
            GetComponentInUnit<CharacterMana>().ManaStat.OnChangedMaximumValue -= OnChangedMana;
            GetComponentInUnit<CharacterHealth>().OnZeroHealth -= OnZeroHealth;
        }

        private void OnChangedEndurance()
        {
            playerUI.OnChangedEndurance(characterEndurance.EnduranceStat.CurrentValue, characterEndurance.EnduranceStat.MaximumValue);
        }
        private void OnChangedHealth()
        {
            playerUI.OnChangedHealth(playerHealth.HealthStat.CurrentValue, playerHealth.HealthStat.MaximumValue);
        }
        private void OnChangedMana()
        {
            playerUI.OnChangedMana(playerMana.ManaStat.CurrentValue, playerMana.ManaStat.MaximumValue);
        }
        
        public override void Activate()
        {
            base.Activate();
            unitTransformSync.enabled = true;
        }

        public override void Appear()
        {
            
        }

        public override void Disappear()
        {
            throw new NotImplementedException();
        }

        //Test
        private void InitializeSword()
        {
            if (!photonView.IsMine) return;
            
            var item = inventoryItemFactory.CreateItem(soNormalSwordItem);
            diContainer.Inject(item);
            item.SetOwner(gameObject);
            item.SetAmountItem(1);
            item.SetStats(soNormalSwordItem.UnitStatsConfigs);
            item.Initialize();
            playerItemInventory.AddItem(item, soNormalSwordItem.Icon);
            var equipmentItem = (EquipmentItem)item;
        }
        
        private void Update()
        {
            if(!IsActive || !photonView.IsMine) return;
            
            playerControlDesktop?.HandleHotkey();
            playerControlDesktop?.HandleInput();
            StateMachine?.Update();
        }

        private void LateUpdate()
        {
            if(!IsActive || !photonView.IsMine) return;
           // Debug.Log(StateMachine.GetState<PlayerAttackState>().IsFindUnitInRange());
            StateMachine?.LateUpdate();
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

        public void ActivateControl()
        {
            playerControlDesktop.ActivateControl();
        }

        public void DeactivateControl()
        {
            playerControlDesktop.DeactivateControl();
        }
    }
}
