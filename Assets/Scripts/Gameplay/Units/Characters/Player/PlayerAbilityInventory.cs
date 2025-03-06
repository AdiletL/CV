using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Factory;
using Gameplay.Ability;
using Gameplay.UI.ScreenSpace.Inventory;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Unit.Character.Player
{
    [RequireComponent(typeof(AbilityHandler))]
    public class PlayerAbilityInventory : MonoBehaviour, IInventory
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeys;
        
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SO_PlayerAbilityInventory so_PlayerAbilityInventory;
        [SerializeField] private SO_PlayerAbilities so_PlayerAbilities;
        [SerializeField] private AssetReferenceGameObject uiAbilityInventoryPrefab;
        
        private AbilityFactory abilityFactory;
        private AbilityHandler abilityHandler;
        private UIAbilityInventory uiAbilityInventory;
        private PlayerBlockInput playerBlockInput;

        private InputType baseBlockInputType;
        private KeyCode[] abilityInventoryKeys;

        private Ability currentSelectedAbility;
        private Ability currentUseAbility;
        private Texture2D selectedAbilityCursor;
        
        private int maxSlot;
        
        private Dictionary<int?, Ability> slots;
        
        public bool IsFullInventory()
        {
            return slots != null && !slots.ContainsValue(null);
        }

        private AbilityFactory CreateAbilityFactory()
        {
            return new AbilityFactoryBuilder()
                .SetMoveControl(GetComponent<IMoveControl>())
                .SetBaseCamera(playerController.BaseCamera)
                .SetGameObject(gameObject)
                .Build();
        }

        public void Initialize()
        {
            baseBlockInputType = so_PlayerAbilityInventory.BaseBlockInputType;
            maxSlot = so_PlayerAbilityInventory.MaxSlot;
            abilityHandler = GetComponent<AbilityHandler>();
            playerBlockInput = playerController.PlayerBlockInput;
            
            InitializeUIInventory();
            InitializeAbilityFactory();
            InitializeKeys();
            InitializeSlots();
        }

        private void OnEnable()
        {
            PlayerControlDesktop.OnBlockInput += OnBlockInput;
            PlayerMouseInputHandler.OnBlockInput += OnBlockInput;
        }

        private void OnDisable()
        {
            PlayerControlDesktop.OnBlockInput -= OnBlockInput;
            PlayerMouseInputHandler.OnBlockInput -= OnBlockInput;
        }

        private  void InitializeUIInventory()
        {
            var handle = Addressables.InstantiateAsync(uiAbilityInventoryPrefab).WaitForCompletion();
            uiAbilityInventory = handle.GetComponent<UIAbilityInventory>();
            diContainer.Inject(uiAbilityInventory);
            uiAbilityInventory.OnClickedLeftMouse += OnClickedLeftMouse;
            uiAbilityInventory.CreateCells(maxSlot);
        }
        private void InitializeAbilityFactory()
        {
            abilityFactory = CreateAbilityFactory();
            diContainer.Inject(abilityFactory);
        }
        private void InitializeKeys()
        {
            abilityInventoryKeys = new KeyCode[so_GameHotkeys.AbilityInventoryKeys.Length];
            for (int i = 0; i < abilityInventoryKeys.Length; i++)
                abilityInventoryKeys[i] = so_GameHotkeys.AbilityInventoryKeys[i];
        }

        private void InitializeSlots()
        {
            slots ??= new ();
            for (int i = 0; i < maxSlot; i++)
                slots.Add(i, null);
        }

        public void AddAbility(AbilityConfig abilityConfig)
        {
            if (slots.Values.Any(a => a != null && a.AbilityType == abilityConfig.SO_BaseAbilityConfig.AbilityType)) return;

            int? slotID = slots.FirstOrDefault(pair => pair.Value == null).Key;
            if (slotID == null) return;

            var newAbility = abilityFactory.CreateAbility(abilityConfig);
            diContainer.Inject(newAbility);
            newAbility.SetInventorySlotID(slotID);
            newAbility.Initialize();
            newAbility.OnCountCooldown += OnCountCooldownAbility;
            newAbility.OnStartedCast += OnStartedCastAbility;
            newAbility.OnFinishedCast += OnFinishedCastAbility;
            newAbility.OnExit += OnExitAbility;
            abilityHandler.AddAbility(newAbility);
            slots[slotID.Value] = newAbility;

            var baseAbilityConfig = abilityConfig.SO_BaseAbilityConfig;
            var isSelectable = (baseAbilityConfig.AbilityBehaviour & AbilityBehaviour.Passive) == 0;
            uiAbilityInventory.AddAbility(slotID, baseAbilityConfig.Icon, isSelectable, 0, abilityConfig.Cooldown);
        }

        public void RemoveAbility(int? slotID)
        {
            if (slotID == null) return;

            slots[slotID].OnCountCooldown -= OnCountCooldownAbility;
            slots[slotID].OnStartedCast -= OnStartedCastAbility;
            slots[slotID].OnFinishedCast -= OnFinishedCastAbility;
            slots[slotID].OnExit -= OnExitAbility;
            
            abilityHandler.RemoveAbilityByID(slots[slotID].AbilityType, slotID);
            uiAbilityInventory.RemoveAbility(slotID);
            slots[slotID.Value] = null;
        }

        private void ClearSelectedAbility()
        {
            ExitAbility(currentSelectedAbility);
            currentSelectedAbility = null;
        }
        
        private void ClearUseAbility()
        {
            ExitAbility(currentUseAbility);
            currentUseAbility = null;
        }
        
        private void OnCountCooldownAbility(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiAbilityInventory.UpdateCooldown(slotID, current, max);
            uiAbilityInventory.UpdateSelectable(slotID, current <= 0);
        }

        private void OnClickedLeftMouse(int? slotID)
        {
            if (!playerBlockInput.IsInputBlocked(InputType.Ability))
            {
                EnterAbility(slotID);
            }
        }
        
        private void OnBlockInput(InputType input)
        {
            ClearUseAbility();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                AddAbility(so_PlayerAbilities.AbilityConfigData.DashConfig);
            }
            if (currentSelectedAbility != null && Input.anyKeyDown &&
                playerBlockInput.IsInputBlocked(InputType.Item))
            {
                ClearSelectedAbility();
            }

            for (int i = 0; i < abilityInventoryKeys.Length; i++)
            {
                if (Input.GetKeyDown(abilityInventoryKeys[i]))
                {
                    if (!playerBlockInput.IsInputBlocked(InputType.Ability))
                    {
                        EnterAbility(i);
                    }
                }
            }
        }

        private void EnterAbility(int? slotID)
        {
            if(slotID == null || slots[slotID] == null) return;
            if(slots[slotID] == currentUseAbility) return;
            
            ExitAbility(currentSelectedAbility);
            currentSelectedAbility = slots[slotID];
            currentSelectedAbility.OnActivated += OnActivatedAbility;
            currentSelectedAbility.Enter();
        }

        private void ExitAbility(Ability ability)
        {
            if(ability == null) return;
            ability.Exit();
            ability.OnActivated -= OnActivatedAbility;
        }

        private void OnActivatedAbility(int? slotID)
        {
            if(slotID == null) return;
            playerBlockInput.BlockInput(baseBlockInputType);
            slots[slotID].OnActivated -= OnActivatedAbility;
        }
        private void OnStartedCastAbility(int? slotID)
        {
            if(slotID == null) return;
            playerBlockInput.BlockInput(slots[slotID].BlockedInputType);
            ExitAbility(currentUseAbility);
            currentUseAbility = slots[slotID];
            currentSelectedAbility = null;
        }
        private void OnFinishedCastAbility(int? slotID)
        {
            if(slotID == null) return;
            playerBlockInput.UnblockInput(slots[slotID].BlockedInputType);
        }
        private void OnExitAbility(int? slotID)
        {
            if(slotID == null) return;
            playerBlockInput.UnblockInput(baseBlockInputType);
            currentUseAbility = null;
        }
    }
}