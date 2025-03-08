using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIItemInventory : UIInventory
    {
        public event Action<int?> OnClickedLeftMouse;
        public event Action<int?> OnClickedRightMouse;
        
        [SerializeField] private AssetReferenceGameObject uiItemPrefab;
        [SerializeField] private Transform container;
        
        private Dictionary<int?, UIItem> uiItems = new ();
        
        public override void CreateCells(int maxCountItem)
        {
            UIItem uiItem = null;
            for (int i = 0; i < maxCountItem; i++)
            {
                var handle = Addressables.InstantiateAsync(uiItemPrefab, container).WaitForCompletion();
                uiItem = handle.GetComponent<UIItem>();
                uiItem.OnClickedLeftMouse += OnClickedLeftMouse;
                uiItem.OnClickedRightMouse += OnClickedRightMouse;
                uiItem.Initialize();
                uiItem.Clear();
                uiItems.Add(i, uiItem);
            }
        }
        
        private void OnLeftMouseClickItem(int? slotID) => OnClickedLeftMouse?.Invoke(slotID);
        private void OnRightMouseClickItem(int? slotID) => OnClickedRightMouse?.Invoke(slotID);

        public void AddItem(int? slotID, Sprite icon, int amount, bool isSelectable, float currentCooldown, float maxCooldown)
        {
            if(slotID == null) return;
            
            uiItems[slotID].SetSlotID(slotID);
            uiItems[slotID].UpdateIcon(icon);
            uiItems[slotID].UpdateAmount(amount);
            uiItems[slotID].UpdateCooldownBar(currentCooldown, maxCooldown);
            uiItems[slotID].UpdateSelectable(isSelectable);
        }

      
        public void UpdateItemCooldown(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiItems[slotID].UpdateCooldownBar(current, max);
        }

        public void UpdateAmount(int? slotID, int amount)
        {
            if(slotID == null) return;
            uiItems[slotID].UpdateAmount(amount);
        }

        public void UpdateSelectable(int? slotID, bool isSelectable)
        {
            if(slotID == null) return;
            uiItems[slotID].UpdateSelectable(isSelectable);
        }

        public void RemoveItem(int? slotID)
        {
            if(slotID == null) return;
            uiItems[slotID].Clear();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < uiItems.Count; i++)
            {
                uiItems[i].OnClickedLeftMouse -= OnLeftMouseClickItem;
                uiItems[i].OnClickedRightMouse -= OnRightMouseClickItem;
            }
        }
    }
}