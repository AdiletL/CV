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
        

        private InputType baseBlockInputType;
        private KeyCode[] abilityInventoryKeys;

        private IAbility currentSelectedAbility;
        
        private int maxSlot;
        
        private Dictionary<InputType, int> blockedInputs = new();
        private Dictionary<int?, AbilityData> slots = new();
        
        public bool IsFullInventory()
        {
            return !slots.ContainsValue(null);
        }

        private AbilityFactory CreateAbilityFactory()
        {
            return new AbilityFactoryBuilder()
                .SetMoveControl(GetComponent<IMoveControl>())
                .SetBaseCamera(playerController.BaseCamera)
                .SetGameObject(gameObject)
                .Build();
        }
        
        public bool IsInputBlocked(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0) continue;

                if (blockedInputs.ContainsKey(flag) && blockedInputs[flag] > 0)
                    return true;
            }
            return false;
        }
        
        public void BlockInput(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0) continue;

                blockedInputs.TryAdd(flag, 0);
                blockedInputs[flag]++;
            }
        }
        
        public void UnblockInput(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0) continue;

                if (blockedInputs.ContainsKey(flag))
                {
                    blockedInputs[flag]--;

                    if (blockedInputs[flag] <= 0) 
                        blockedInputs.Remove(flag);
                }
            }
        }
        
        public void Initialize()
        {
            baseBlockInputType = so_PlayerAbilityInventory.BaseBlockInputType;
            maxSlot = so_PlayerAbilityInventory.MaxSlot;
            abilityHandler = GetComponent<AbilityHandler>();
            
            InitializeUIInventory();
            InitializeAbilityFactory();
            InitializeKeys();
            InitializeSlots();
        }

        private void OnEnable()
        {
            UIAbility.OnSlotSelected += OnSlotSelected;
        }

        private void OnDisable()
        {
            UIAbility.OnSlotSelected -= OnSlotSelected;
        }

        private  void InitializeUIInventory()
        {
            var handle = Addressables.InstantiateAsync(uiAbilityInventoryPrefab).WaitForCompletion();
            uiAbilityInventory = handle.GetComponent<UIAbilityInventory>();
            diContainer.Inject(uiAbilityInventory);
            uiAbilityInventory.SetMaxCountCells(maxSlot);
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
            abilityHandler.AddAbility(newAbility);

            var baseAbilityConfig = abilityConfig.SO_BaseAbilityConfig;
            var abilityData = new AbilityData(slotID, baseAbilityConfig.AbilityType, baseAbilityConfig.AbilityBehaviour, baseAbilityConfig.BlockedInputType, baseAbilityConfig.Icon);
            uiAbilityInventory.AddAbility(abilityData);
            slots[slotID.Value] = abilityData;
        }

        public void RemoveAbility(AbilityData abilityData)
        {
            int? slotID = slots.FirstOrDefault(pair => pair.Value?.AbilityType == abilityData.AbilityType).Key;
            if (slotID == null) return;

            abilityHandler.GetAbility(abilityData.AbilityType, slotID).OnCountCooldown -= OnCountCooldownAbility;
            abilityHandler.RemoveAbilityByID(abilityData.AbilityType, slotID);
            uiAbilityInventory.RemoveAbility(abilityData.SlotID);
            slots[slotID.Value] = null;
        }

        private void OnCountCooldownAbility(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiAbilityInventory.UpdateCooldown(slotID, current, max);
            uiAbilityInventory.ChangeReadiness(slotID, current <= 0);
        }

        private void OnSlotSelected(int? slotID)
        {
            EnterAbility(slotID);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                AddAbility(so_PlayerAbilities.DashConfig);
            }

            for (int i = 0; i < abilityInventoryKeys.Length; i++)
            {
                if (Input.GetKeyDown(abilityInventoryKeys[i]))
                {
                    EnterAbility(i);
                }
            }
        }

        private void EnterAbility(int? slotID)
        {
            if(slotID == null || slots[slotID] == null) return;
            ExitAbility(currentSelectedAbility);
            currentSelectedAbility = null;
            
            currentSelectedAbility = abilityHandler.GetAbility(slots[slotID].AbilityType, slots[slotID].SlotID);
            currentSelectedAbility.OnActivated += OnActivatedAbility;
            currentSelectedAbility.Enter();
        }

        private void ExitAbility(IAbility ability)
        {
            if(ability == null) return;
            ability.Exit();
            ability.OnActivated -= OnActivatedAbility;
            ability.OnStarted -= OnStartedAbility;
            ability.OnFinished -= OnFinishedAbility;
            ability.OnExit -= OnExitAbility;
        }

        private void OnActivatedAbility(int? slotID)
        {
            if(slotID == null) return;
            BlockInput(baseBlockInputType);
            var ability = abilityHandler.GetAbility(slots[slotID].AbilityType, slots[slotID].SlotID);
            ability.OnActivated -= OnActivatedAbility;
            ability.OnStarted += OnStartedAbility;
            ability.OnFinished += OnFinishedAbility;
            ability.OnExit += OnExitAbility;
        }
        private void OnStartedAbility(int? slotID)
        {
            if(slotID == null) return;
            BlockInput(slots[slotID].BlockedInputType);
            abilityHandler.GetAbility(slots[slotID].AbilityType, slots[slotID].SlotID).OnStarted -= OnStartedAbility;
            currentSelectedAbility = null;
        }
        private void OnFinishedAbility(int? slotID)
        {
            if(slotID == null) return;
            UnblockInput(slots[slotID].BlockedInputType);
            abilityHandler.GetAbility(slots[slotID].AbilityType, slots[slotID].SlotID).OnFinished -= OnFinishedAbility;
        }
        private void OnExitAbility(int? slotID)
        {
            if(slotID == null) return;
            UnblockInput(baseBlockInputType);
            var ability = abilityHandler.GetAbility(slots[slotID].AbilityType, slots[slotID].SlotID);
            ability.OnActivated -= OnActivatedAbility;
            ability.OnExit -= OnExitAbility;
        }
    }
}