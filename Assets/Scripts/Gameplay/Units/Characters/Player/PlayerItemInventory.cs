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
        
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SO_PlayerItemInventory so_PlayerItemInventory;
        [SerializeField] private AssetReferenceGameObject uiItemInventoryPrefab;

        private ItemFactory itemFactory;
        private ItemHandler itemHandler;
        private UIItemInventory uiItemInventory;
        private PlayerAbilityInventory playerAbilityInventory;

        private Gameplay.Units.Item.Item currentSelectedItem;
        private InputType baseBlockInput;
        
        private int maxSlot;
        
        private Dictionary<InputType, int> blockedInputs;
        private Dictionary<int?, Gameplay.Units.Item.Item> slots;
        
        public bool IsFullInventory()
        {
            return slots != null && !slots.ContainsValue(null);
        }
        
        public bool IsNotNullItem(ItemName itemNameID)
        {
            if (slots == null) return false;
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
            if (slots == null) return null;
            foreach (var VARIABLE in slots.Values)
            {
                if(VARIABLE.ItemNameID == itemNameID)
                    return VARIABLE;
            }
            return null;
        }
        
        public bool IsInputBlocked(InputType input)
        {
            if(blockedInputs == null) return false;
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
            blockedInputs ??= new();
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0) continue;

                blockedInputs.TryAdd(flag, 0);
                blockedInputs[flag]++;
            }
        }
        
        public void UnblockInput(InputType input)
        {
            if(blockedInputs == null) return;
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
            itemHandler = GetComponent<ItemHandler>();
            playerAbilityInventory = GetComponent<PlayerAbilityInventory>();
            
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
            slots ??= new();
            for (int i = 0; i < maxSlot; i++)
                slots.Add(i, null);
        }

        public void AddItem(SO_Item so_Item, int amount)
        {
            int? slotID = null;
            
            if (!IsNotNullItem(so_Item.ItemNameID))
            {
                slotID = slots.FirstOrDefault(pair => pair.Value == null).Key;
                if(slotID == null) return;
                
                var newItem = itemFactory.CreateItem(so_Item);
                if(newItem == null) return;
                
                diContainer.Inject(newItem);
                newItem.SetInventorySlotID(slotID);
                newItem.SetAmountItem(amount);
                newItem.Initialize();
                
                slots[slotID] = newItem;
                itemHandler.AddItem(newItem);
                newItem.OnCountCooldown += OnCountCooldownItem;
                
                bool isSelectable = (so_Item.ItemBehaviourID & ItemBehaviour.Passive) == 0;
                uiItemInventory.AddItem(slotID, so_Item.Icon, amount, isSelectable, 0, so_Item.Cooldown);
            }
            else
            {
                slotID = slots.FirstOrDefault(pair => string.Equals(so_Item.ItemNameID, pair.Value.ItemNameID)).Key;
                if(slotID == null) return;
                
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
                OnExitItem(slotID);
                itemHandler.RemoveItemByID(slots[slotID].ItemNameID, slotID);
                uiItemInventory.RemoveItem(slotID);
                slots[slotID] = null;
            }
        }
        
        private void ClearSelectedItem()
        {
            ExitItem(currentSelectedItem);
            currentSelectedItem = null;
        }
        
        private void OnSlotSelected(int? slotID)
        {
            if (!IsInputBlocked(InputType.Item) &&
                !playerAbilityInventory.IsInputBlocked(InputType.Item))
            {
                EnterItem(slotID);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                ClearSelectedItem();
            }
        }

        private void EnterItem(int? slotID)
        {
            if(slotID == null || slots[slotID] == null) return;
            ExitItem(currentSelectedItem);

            currentSelectedItem = slots[slotID];
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
            slots[slotID].OnActivated -= OnActivatedItem;
            slots[slotID].OnStarted += OnStartedItem;
            slots[slotID].OnFinished += OnFinishedItem;
            slots[slotID].OnExit += OnExitItem;
        }
        private void OnStartedItem(int? slotID)
        {
            if(slotID == null) return;
            BlockInput(slots[slotID].BlockInputType);
            slots[slotID].OnStarted -= OnStartedItem;
            currentSelectedItem = null;
        }
        private void OnFinishedItem(int? slotID)
        {
            if(slotID == null) return;
            UnblockInput(slots[slotID].BlockInputType);
            slots[slotID].OnFinished -= OnFinishedItem;
            RemoveItem(slotID, 1);
        }
        private void OnExitItem(int? slotID)
        {
            if(slotID == null) return;
            UnblockInput(baseBlockInput);
            
            if(slots[slotID] == null) return;
            slots[slotID].OnActivated -= OnActivatedItem;
            slots[slotID].OnExit -= OnExitItem;
        }

        private void OnCountCooldownItem(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiItemInventory.UpdateItemCooldown(slotID, current, max);
            uiItemInventory.UpdateSelectable(slotID, current <= 0);
        }
    }
}