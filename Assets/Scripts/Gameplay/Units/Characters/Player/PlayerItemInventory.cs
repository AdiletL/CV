using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Factory;
using Gameplay.Ability;
using Gameplay.UI.ScreenSpace.Inventory;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character.Player;
using Unit.Item;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerItemInventory : MonoBehaviour, IInventory
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameConfig so_GameConfig;
        
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SO_PlayerItemInventory soPlayerItemInventory;
        [SerializeField] private AssetReferenceGameObject uiItemInventoryPrefab;
        
        private UIItemInventory uiItemInventory;
        private AbilityFactory abilityFactory;
        private AbilityHandler abilityHandler;
        
        private ItemData currentSelectedItem;
        private List<IAbility> currentAbilities = new();

        private Texture2D selectedItemCursor;
        private int maxSlot;
        
        private Dictionary<InputType, int> blockedInputs = new();
        private Dictionary<int?, ItemData> slots = new();
        
        public bool IsFullInventory()
        {
            return !slots.ContainsValue(null);
        }
        
        public bool IsNotNullItem(string name)
        {
            foreach (var VARIABLE in slots.Values)
            {
                if(VARIABLE != null && 
                   VARIABLE.Name == name)
                    return true;
            }

            return false;
        }

        public ItemData GetItem(string name)
        {
            foreach (var VARIABLE in slots.Values)
            {
                if(VARIABLE.Name == name)
                    return VARIABLE;
            }
            return null;
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

        private AbilityFactory CreateSkillFactory()
        {
            return new SkillFactoryBuilder(new AbilityFactory())
                .SetBaseCamera(playerController.BaseCamera)
                .SetGameObject(gameObject)
                .Build();
        }
        
        public void OnEnable()
        {
            UIItem.OnSlotSelected += OnSlotSelected;
        }
        public void OnDisable()
        {
            UIItem.OnSlotSelected -= OnSlotSelected;
        }

        public void Initialize()
        {
            maxSlot = soPlayerItemInventory.MaxCountItem;
            selectedItemCursor = so_GameConfig.SelectedItemCursor;
            abilityHandler = GetComponent<AbilityHandler>();
            InitializeUIInventory();
            InitializeSkillFactory();
            InitializeSlots();
        }

        private  void InitializeUIInventory()
        {
            var handle = Addressables.InstantiateAsync(uiItemInventoryPrefab).WaitForCompletion();
            uiItemInventory = handle.GetComponent<UIItemInventory>();
            diContainer.Inject(uiItemInventory);
            uiItemInventory.SetMaxCountCells(maxSlot);
        }

        private void InitializeSkillFactory()
        {
            abilityFactory = CreateSkillFactory();
            diContainer.Inject(abilityFactory);
        }

        private void InitializeSlots()
        {
            for (int i = 0; i < maxSlot; i++)
                slots.Add(i, null);
        }

        public void AddItem(ItemData data)
        {
            int? slotID = slots.FirstOrDefault(pair => pair.Value == null || string.Equals(data.Name, pair.Value.Name)).Key;
            if (slotID == null) return;

            if (!IsNotNullItem(data.Name))
            {
                slots[slotID] = data;
                data.SetSlotID(slotID);
                AddAbilities(data.AbilityConfigs, slotID);
                uiItemInventory.AddItem(slots[slotID]);
            }
            else
            {
                slots[slotID].Amount += data.Amount;
                uiItemInventory.UpdateItem(slots[slotID]);
            }
        }

        public void RemoveItem(ItemData data)
        {
            if(data.SlotID == null || data.Amount > slots[data.SlotID].Amount) return;
            
            slots[data.SlotID].Amount -= 1;
            
            if (slots[data.SlotID].Amount <= 0)
            {
                uiItemInventory.RemoveItem(data.SlotID);
                slots.Remove(data.SlotID);
                Debug.Log(data.AbilityConfigs.Count);
                foreach (var VARIABLE in data.AbilityConfigs)
                    RemoveAbilities(VARIABLE.AbilityType, data.SlotID);
            }
            else
            {
                uiItemInventory.UpdateItem(data);
            }
        }

        private void AddAbilities(List<AbilityConfig> skillAbilities, int? id)
        {
            foreach (var VARIEBLE in skillAbilities)
            {
                var newAbility = abilityFactory.CreateAbility(VARIEBLE);
                if (newAbility != null)
                {
                    diContainer.Inject(newAbility);
                    newAbility.SetSlotID(id);
                    newAbility.Initialize();
                    newAbility.OnCountCooldown += OnCountCooldownAbility;
                    abilityHandler.AddAbility(newAbility);
                }
            }
        }

        private void RemoveAbilities(AbilityType abilityTypes, int? id)
        {
            foreach (AbilityType abilityType in Enum.GetValues(typeof(AbilityType)))
            {
                if (abilityType == AbilityType.Nothing) continue;
                
                if (abilityTypes.HasFlag(abilityType))
                {
                    abilityHandler.GetAbility(abilityType, id).OnCountCooldown -= OnCountCooldownAbility;
                    abilityHandler.RemoveAbilityByID(abilityType, id);
                }
            }
        }

        private void OnSlotSelected(int? slotID)
        {
            if(slotID == null) return;
            
            currentSelectedItem = slots[slotID];
            foreach (var VARIABLE in currentSelectedItem.AbilityConfigs)
            {
                var ability = abilityHandler.GetAbility(VARIABLE.AbilityType, currentSelectedItem.SlotID);
                if (ability != null)
                {
                    currentAbilities.Add(ability);
                    BlockInput(VARIABLE.BlockedInputType);
                }
            }
            
            Cursor.SetCursor(selectedItemCursor, Vector2.zero, CursorMode.Auto);
        }

        public void ClearSelectedItem()
        {
            if(currentSelectedItem == null) return;
            foreach (var VARIABLE in currentSelectedItem.AbilityConfigs)
                UnblockInput(VARIABLE.BlockedInputType);
            
            currentAbilities.Clear();
            currentSelectedItem = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        private void OnCountCooldownAbility(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiItemInventory.UpdateItemCooldown(slotID, current, max);
            uiItemInventory.ChangeReadiness(slotID, current <= 0);
        }
        private void Update()
        {
            if (currentSelectedItem != null && 
                Input.GetMouseButtonDown(0))
            {
                for (int i = currentAbilities.Count - 1; i >= 0; i--)
                {
                    abilityHandler.Activate(currentAbilities[i].AbilityType, currentSelectedItem.SlotID);
                    if (i == 0)
                    {
                        RemoveItem(currentSelectedItem);
                        ClearSelectedItem();
                    }
                }
            }
        }
    }
}