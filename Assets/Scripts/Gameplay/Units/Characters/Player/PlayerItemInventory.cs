using System;
using System.Collections.Generic;
using System.Linq;
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
    public class PlayerItemInventory : MonoBehaviour, IInventory
    {
        [Inject] private DiContainer diContainer;

        [SerializeField] private PlayerController playerController;
        [SerializeField] private SO_PlayerItemInventory so_PlayerItemInventory;
        [SerializeField] private AssetReferenceGameObject uiItemInventoryPrefab;

        private ItemHandler itemHandler;
        private UIItemInventory uiItemInventory;
        private PlayerBlockInput playerBlockInput;

        private Item.Item currentSelectedItem;
        private Item.Item currentUseItem;
        private Camera baseCamera;
        private InputType baseBlockInput;
        
        private int maxSlot;
        private bool isNextFrameFromUnblockInput;
        
        private Dictionary<int?, Item.Item> slots;
        
        
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

        public Item.Item GetItem(ItemName itemNameID)
        {
            if (slots == null) return null;
            foreach (var VARIABLE in slots.Values)
            {
                if(VARIABLE.ItemNameID == itemNameID)
                    return VARIABLE;
            }
            return null;
        }

        public void Initialize()
        {
            baseBlockInput = so_PlayerItemInventory.BaseBlockInputType;
            maxSlot = so_PlayerItemInventory.MaxCountItem;
            itemHandler = GetComponent<ItemHandler>();
            playerBlockInput = playerController.PlayerBlockInput;
            baseCamera = playerController.BaseCamera;
            
            InitializeUIInventory();
            InitializeSlots();
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

        public void AddItem(Item.Item item, Sprite icon)
        {
            int? slotID = null;
            
            if (!IsNotNullItem(item.ItemNameID))
            {
                slotID = slots.FirstOrDefault(pair => pair.Value == null).Key;
                if(slotID == null) return;
                
                item.SetInventorySlotID(slotID);
                item.AddStatsFromUnit();
                
                slots[slotID] = item;
                itemHandler.AddItem(item);
                item.OnCountCooldown += OnCountCooldownItem;
                item.OnStartedCast += OnStartedCastItem;
                item.OnFinishedCast += OnFinishedCastItem;
                
                bool isSelectable = item.ItemBehaviourID != ItemBehaviour.Passive;
                uiItemInventory.AddItem(slotID, icon, item.Amount, isSelectable, 0, 0);
            }
            else
            {
                slotID = slots.FirstOrDefault(pair => Equals(item.ItemNameID, pair.Value.ItemNameID)).Key;
                if(slotID == null) return;
                
                slots[slotID].AddAmount(item.Amount);
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
                slots[slotID].OnStartedCast -= OnStartedCastItem;
                slots[slotID].OnFinishedCast -= OnFinishedCastItem;
                
                itemHandler.RemoveItemByID(slots[slotID].ItemNameID, slotID);
                uiItemInventory.RemoveItem(slotID);
                slots[slotID].RemoveStatsFromUnit();
                slots[slotID] = null;
            }
        }
        
        private void OnClickedLeftMouseInventory(int? slotID)
        {
            if (!playerBlockInput.IsInputBlocked(InputType.Item))
            {
                switch (slots[slotID].ItemBehaviourID)
                {
                    case ItemBehaviour.NoTarget: slots[slotID].Enter(); break;
                    case ItemBehaviour.PointTarget: 
                    case ItemBehaviour.UnitTarget:
                        currentSelectedItem = slots[slotID]; 
                        playerBlockInput.BlockInput(baseBlockInput);
                        break;
                }
            }
        }

        private void OnClickedRightMouseInventory(int? slotID)
        {
            if(slotID == null || slots[slotID] == null) return;
            slots[slotID].ShowContextMenu();
        }
        
        private void OnCountCooldownItem(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiItemInventory.UpdateItemCooldown(slotID, current, max);
            uiItemInventory.UpdateSelectable(slotID, current <= 0);
        }
        
        private void ClearSelectedItem()
        {
            ExitItem(currentSelectedItem);
            currentSelectedItem = null;
        }

        private void ClearUseItem()
        {
            if(currentUseItem == null) return;
            playerBlockInput.UnblockInput(slots[currentUseItem.InventorySlotID].BlockInputTypeID);
            ExitItem(currentUseItem);
            currentUseItem = null;
        }
        
        private void Update()
        {
            if (isNextFrameFromUnblockInput)
            {
                playerBlockInput.UnblockInput(baseBlockInput);
                isNextFrameFromUnblockInput = false;
            }
            
            if (currentUseItem != null)
            {
                if (Input.anyKeyDown && !CheckInputOnUI.IsPointerOverUIObject())
                {
                    ClearUseItem();
                }
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
                    currentSelectedItem = null;
                    isNextFrameFromUnblockInput = true;
                }
            }
        }
        
        private void OnStartedCastItem(int? slotID)
        {
            if(slotID == null) return;
            ExitItem(currentUseItem);
            playerBlockInput.BlockInput(slots[slotID].BlockInputTypeID);
            currentUseItem = slots[slotID];
        }
        private void OnFinishedCastItem(int? slotID)
        {
            if(slotID == null) return;
            playerBlockInput.UnblockInput(slots[slotID].BlockInputTypeID);
            currentUseItem = null;
            if(!(slots[slotID] is EquipmentItem))
                RemoveItem(slotID, 1);
        }
        private void ExitItem(Item.Item item)
        {
            item?.Exit();
        }
    }
}