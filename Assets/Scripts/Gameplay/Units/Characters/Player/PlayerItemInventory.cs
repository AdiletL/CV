using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Factory;
using Gameplay.Ability;
using Gameplay.UI.ScreenSpace.Inventory;
using Gameplay.Units.Item;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character.Player;
using ScriptableObjects.Unit.Item;
using Unit.Item;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Zenject;

namespace Unit.Character.Player
{
    [RequireComponent(typeof(ItemHandler))]
    public class PlayerItemInventory : MonoBehaviour, IInventory
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameConfig so_GameConfig;
        
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SO_PlayerItemInventory so_PlayerItemInventory;
        [SerializeField] private AssetReferenceGameObject uiItemInventoryPrefab;

        private ItemFactory itemFactory;
        private ItemHandler itemHandler;
        private UIItemInventory uiItemInventory;

        private Gameplay.Units.Item.Item currentSelectedItem;
        private InputType baseBlockInput;
        
        private Texture2D selectedItemCursor;
        private int maxSlot;
        
        private Dictionary<InputType, int> blockedInputs = new();
        private Dictionary<int?, Gameplay.Units.Item.Item> slots = new();
        
        public bool IsFullInventory()
        {
            return !slots.ContainsValue(null);
        }
        
        public bool IsNotNullItem(ItemName itemNameID)
        {
            foreach (var VARIABLE in slots.Values)
            {
                if(VARIABLE != null && 
                   VARIABLE.ItemNameID == itemNameID)
                    return true;
            }

            return false;
        }

        public Gameplay.Units.Item.Item GetItem(ItemName itemNameID)
        {
            foreach (var VARIABLE in slots.Values)
            {
                if(VARIABLE.ItemNameID == itemNameID)
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

        private ItemFactory CreateItemFactory()
        {
            AbilityFactory abilityFactory = new AbilityFactoryBuilder()
                .SetMoveControl(GetComponent<IMoveControl>())
                .SetBaseCamera(playerController.BaseCamera)
                .SetGameObject(gameObject)
                .Build();
            diContainer.Inject(abilityFactory);
            
            return new ItemFactoryBuilder()
                .SetAbilityFactory(abilityFactory)
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
            baseBlockInput = so_PlayerItemInventory.BaseBlockInputType;
            maxSlot = so_PlayerItemInventory.MaxCountItem;
            selectedItemCursor = so_GameConfig.SelectedItemCursor;
            itemHandler = GetComponent<ItemHandler>();
            InitializeUIInventory();
            InitializeItemFactory();
            InitializeSlots();
        }

        private  void InitializeUIInventory()
        {
            var handle = Addressables.InstantiateAsync(uiItemInventoryPrefab).WaitForCompletion();
            uiItemInventory = handle.GetComponent<UIItemInventory>();
            diContainer.Inject(uiItemInventory);
            uiItemInventory.SetMaxCountCells(maxSlot);
        }

        private void InitializeItemFactory()
        {
            itemFactory = CreateItemFactory();
            diContainer.Inject(itemFactory);
        }

        private void InitializeSlots()
        {
            for (int i = 0; i < maxSlot; i++)
                slots.Add(i, null);
        }

        public void AddItem(SO_Item so_Item, int amount)
        {
            int? slotID = slots.FirstOrDefault(pair => pair.Value == null || string.Equals(so_Item.ItemNameID, pair.Value.ItemNameID)).Key;
            if(slotID == null) return;
            
            if (!IsNotNullItem(so_Item.ItemNameID))
            {
                var newItem = itemFactory.CreateItem(so_Item);
                if(newItem == null) return;
                
                diContainer.Inject(newItem);
                newItem.SetInventorySlotID(slotID);
                newItem.SetAmountItem(amount);
                newItem.Initialize();
                
                slots[slotID] = newItem;
                itemHandler.AddItem(newItem);
                newItem.OnCountCooldown += OnCountCooldownItem;
                
                bool isReady = (so_Item.ItemBehaviourID & ItemBehaviour.Passive) == 0;
                uiItemInventory.AddItem(slotID, so_Item.Icon, amount, isReady, 0, so_Item.Cooldown);
            }
            else
            {
                slots[slotID].AddAmount(amount);
                uiItemInventory.UpdateAmount(slotID, slots[slotID].Amount);
            }
        }

        public void RemoveItem(int? slotID, int amount)
        {
            if(slotID == null || amount > slots[slotID].Amount) return;
            
            slots[slotID].RemoveAmount(amount);
            uiItemInventory.UpdateAmount(slotID, slots[slotID].Amount);
            
            if (slots[slotID].Amount <= 0)
            {
                slots[slotID].OnCountCooldown -= OnCountCooldownItem;
                itemHandler.RemoveItemByID(slots[slotID].ItemNameID, slotID);
                uiItemInventory.RemoveItem(slotID);
                slots[slotID] = null;
            }
        }
        
        private void OnSlotSelected(int? slotID)
        {
            EnterItem(slotID);
        }

         private void EnterItem(int? slotID)
        {
            if(slotID == null || slots[slotID] == null) return;
            ExitItem(currentSelectedItem);
            currentSelectedItem = null;
            
            currentSelectedItem = itemHandler.GetItem(slots[slotID].ItemNameID, slots[slotID].InventorySlotID);
            currentSelectedItem.OnActivated += OnActivatedItem;
            currentSelectedItem.Enter();
        }

        private void ExitItem(Gameplay.Units.Item.Item item)
        {
            if(item == null) return;
            item.Exit();
            item.OnActivated -= OnActivatedItem;
            item.OnStarted -= OnStartedItem;
            item.OnFinished -= OnFinishedItem;
            item.OnExit -= OnExitItem;
        }

        private void OnActivatedItem(int? slotID)
        {
            if(slotID == null) return;
            BlockInput(baseBlockInput);
            var item = itemHandler.GetItem(slots[slotID].ItemNameID, slots[slotID].InventorySlotID);
            item.OnActivated -= OnActivatedItem;
            item.OnStarted += OnStartedItem;
            item.OnFinished += OnFinishedItem;
            item.OnExit += OnExitItem;
        }
        private void OnStartedItem(int? slotID)
        {
            if(slotID == null) return;
            BlockInput(slots[slotID].BlockInputType);
            itemHandler.GetItem(slots[slotID].ItemNameID, slots[slotID].InventorySlotID).OnStarted -= OnStartedItem;
            currentSelectedItem = null;
        }
        private void OnFinishedItem(int? slotID)
        {
            if(slotID == null) return;
            UnblockInput(slots[slotID].BlockInputType);
            itemHandler.GetItem(slots[slotID].ItemNameID, slots[slotID].InventorySlotID).OnFinished -= OnFinishedItem;
        }
        private void OnExitItem(int? slotID)
        {
            if(slotID == null) return;
            UnblockInput(baseBlockInput);
            var item = itemHandler.GetItem(slots[slotID].ItemNameID, slots[slotID].InventorySlotID);
            item.OnActivated -= OnActivatedItem;
            item.OnExit -= OnExitItem;
            RemoveItem(slotID, 1);
        }
        
        public void ClearSelectedItem()
        {
            currentSelectedItem = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        private void OnCountCooldownItem(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiItemInventory.UpdateItemCooldown(slotID, current, max);
            uiItemInventory.UpdateReadiness(slotID, current <= 0);
        }
    }
}