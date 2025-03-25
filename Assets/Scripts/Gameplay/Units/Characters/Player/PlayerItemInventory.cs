using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Factory.Character.Player;
using Gameplay.UI.ScreenSpace.Inventory;
using Gameplay.Unit.Cell;
using Gameplay.Unit.Item;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Unit.Character.Player
{
    [RequireComponent(typeof(ItemHandler))]
    public class PlayerItemInventory : MonoBehaviour, IInventory, IActivatable
    {
        [Inject] private DiContainer diContainer;

        [SerializeField] private PlayerController playerController;
        [SerializeField] private SO_PlayerItemInventory so_PlayerItemInventory;
        [SerializeField] private SO_PlayerItemUsage so_PlayerItemUsage;
        [SerializeField] private AssetReferenceGameObject uiItemInventoryPrefab;

        private ItemHandler itemHandler;
        private UIItemInventory uiItemInventory;
        private PlayerBlockInput playerBlockInput;
        private PlayerItemUsageState playerItemUsageState;

        private Item.Item currentSelectedItem;
        private Item.Item currentUseItem;
        private Camera baseCamera;
        private InputType selectItemBlockInput;
        
        private int maxSlot;
        private bool isNextFrameFromUnblockInput;
        
        private Dictionary<int?, Item.Item> slots;
        
        public bool IsActive { get; private set; }
        
        public bool IsFullInventory()
        {
            return slots != null && !slots.ContainsValue(null);
        }
        
        public bool IsNotNullItem(string itemName)
        {
            if (slots == null) return false;
            foreach (var VARIABLE in slots.Values)
            {
                if(VARIABLE != null && 
                   string.Equals(VARIABLE.ItemName, itemName))
                    return true;
            }

            return false;
        }

        public Item.Item GetItem(string itemName)
        {
            if (slots == null) return null;
            foreach (var VARIABLE in slots.Values)
            {
                if(string.Equals(VARIABLE.ItemName, itemName))
                    return VARIABLE;
            }
            return null;
        }

        public void Initialize()
        {
            selectItemBlockInput = so_PlayerItemInventory.SelectItemBlockInputType;
            maxSlot = so_PlayerItemInventory.MaxCountItem;
            playerBlockInput = playerController.PlayerBlockInput;
            baseCamera = playerController.BaseCamera;
            itemHandler = GetComponent<ItemHandler>();
            
            InitializeUIInventory();
            InitializeSlots();
            InitializeItemUsageState();
        }

        private  void InitializeUIInventory()
        {
            var handle = Addressables.InstantiateAsync(uiItemInventoryPrefab).WaitForCompletion();
            uiItemInventory = handle.GetComponent<UIItemInventory>();
            diContainer.Inject(uiItemInventory);
            uiItemInventory.OnClickedLeftMouse += OnClickedLeftMouseInventory;
            uiItemInventory.OnClickedRightMouse += OnClickedRightMouseInventory;
            uiItemInventory.CreateCells(maxSlot);
        }

        private void InitializeSlots()
        {
            slots ??= new();
            for (int i = 0; i < maxSlot; i++)
                slots.Add(i, null);
        }

        private void InitializeItemUsageState()
        {
            playerController.PlayerStateFactory.SetPlayerItemUsageConfig(so_PlayerItemUsage);
            playerItemUsageState = (PlayerItemUsageState)playerController.PlayerStateFactory.CreateState(typeof(PlayerItemUsageState));
            diContainer.Inject(playerItemUsageState);
            playerItemUsageState.Initialize();
            playerController.StateMachine.AddStates(playerItemUsageState);
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
        
        public void AddItem(Item.Item item, Sprite icon)
        {
            int? slotID = null;
            
            if (!IsNotNullItem(item.ItemName))
            {
                slotID = slots.FirstOrDefault(pair => pair.Value == null).Key;
                if(slotID == null) return;
                
                item.SetInventorySlotID(slotID);
                item.AddStatsToUnit();
                
                slots[slotID] = item;
                itemHandler.AddItem(item);
                item.OnCountCooldown += OnCountCooldownItem;
                
                bool isSelectable = item.ItemBehaviourID != ItemBehaviour.Passive;
                uiItemInventory.AddItem(slotID, icon, item.Amount, isSelectable, 0, 0);
            }
            else
            {
                slotID = slots.FirstOrDefault(pair => Equals(item.ItemName, pair.Value.ItemName)).Key;
                if(slotID == null) return;
                
                slots[slotID].AddAmount(item.Amount);
                uiItemInventory.SetAmount(slotID, slots[slotID].Amount);
            }
        }

        public void RemoveItem(Item.Item item, int amount)
        {
            if(item == null || amount > item.Amount) return;
            
            item.RemoveAmount(amount);
            uiItemInventory.SetAmount(item.InventorySlotID, item.Amount);
            
            if (item.Amount <= 0)
            {
                item.OnCountCooldown -= OnCountCooldownItem;
                
                itemHandler.RemoveItemByID(item.ItemName, item.InventorySlotID);
                uiItemInventory.RemoveItem(item.InventorySlotID);
                item.RemoveStatsFromUnit();
            }
        }
        
        private void OnClickedLeftMouseInventory(int? slotID)
        {
            if(!IsActive) return;
            if (!playerBlockInput.IsInputBlocked(InputType.Item))
            {
                switch (slots[slotID].ItemBehaviourID)
                {
                    case ItemBehaviour.NoTarget: 
                        slots[slotID].Enter();
                        playerItemUsageState.SetItem(slots[slotID]);
                        playerController.StateMachine.ExitOtherStates(playerItemUsageState.GetType());
                        break;
                    case ItemBehaviour.PointTarget: 
                    case ItemBehaviour.UnitTarget:
                        currentSelectedItem = slots[slotID]; 
                        playerBlockInput.BlockInput(selectItemBlockInput);
                        break;
                }
            }
        }

        private void OnClickedRightMouseInventory(int? slotID)
        {
            if(!IsActive) return;
            if(slotID == null || slots[slotID] == null) return;
            slots[slotID].ShowContextMenu();
        }
        
        private void OnCountCooldownItem(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiItemInventory.SetItemCooldown(slotID, current, max);
            uiItemInventory.SetSelectable(slotID, current <= 0);
        }
        
        private void ClearSelectedItem()
        {
            ExitItem(currentSelectedItem);
            currentSelectedItem = null;
        }
        
        private void Update()
        {
            if(!IsActive) return;
            
            if (isNextFrameFromUnblockInput)
            {
                playerBlockInput.UnblockInput(selectItemBlockInput);
                isNextFrameFromUnblockInput = false;
            }
            
            if (currentSelectedItem != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    switch (currentSelectedItem.ItemBehaviourID)
                    {
                        case ItemBehaviour.PointTarget: PointTarget(); break;
                        case ItemBehaviour.UnitTarget: UnitTarget(); break;
                    }
                }
                else if(Input.GetMouseButtonDown(1))
                {
                    ClearSelectedItem();
                }
            }
        }

        private void PointTarget()
        {
            Ray ray = baseCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, Layers.CELL_LAYER))
            {
                if (hit.collider.TryGetComponent(out CellController cellController) &&
                    !cellController.IsBlocked())
                {
                    currentSelectedItem.Enter(point: hit.point);
                    playerItemUsageState.SetItem(currentSelectedItem);
                    playerController.StateMachine.ExitOtherStates(playerItemUsageState.GetType());
                    currentSelectedItem = null;
                    isNextFrameFromUnblockInput = true;
                }
            }
        }

        private void UnitTarget()
        {
            Ray ray = baseCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, Layers.CREEP_LAYER | Layers.PLAYER_LAYER))
            {
                if (hit.collider.TryGetComponent(out CharacterMainController characterMainController))
                {
                    currentSelectedItem.Enter(target: characterMainController.gameObject);
                    playerItemUsageState.SetItem(currentSelectedItem);
                    playerController.StateMachine.ExitOtherStates(playerItemUsageState.GetType());
                    currentSelectedItem = null;
                    isNextFrameFromUnblockInput = true;
                }
            }
        }

        public void FinishedCastItem(Item.Item item)
        {
            if(item == null) return;
            if(!(item is EquipmentItem))
                RemoveItem(item, 1);
        }
        private void ExitItem(Item.Item item)
        {
            item?.Exit();
        }
    }
}