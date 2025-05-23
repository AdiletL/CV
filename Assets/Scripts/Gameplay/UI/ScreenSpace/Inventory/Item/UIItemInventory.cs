﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIItemInventory : UIInventory
    {
        public event Action<int?> OnClickedLeftMouse;
        public event Action<int?> OnClickedRightMouse;
        public event Action<int?> OnEnter;
        public event Action OnExit;
        
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
                uiItem.OnEnter += OnEnterItem;
                uiItem.OnExit += OnExitItem;
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
            uiItems[slotID].SetIcon(icon);
            uiItems[slotID].SetAmount(amount);
            uiItems[slotID].SetCooldownBar(currentCooldown, maxCooldown);
            uiItems[slotID].SetSelectable(isSelectable);
        }

      
        public void SetItemCooldown(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiItems[slotID].SetCooldownBar(current, max);
        }

        public void SetAmount(int? slotID, int amount)
        {
            if(slotID == null) return;
            uiItems[slotID].SetAmount(amount);
        }

        public void SetSelectable(int? slotID, bool isSelectable)
        {
            if(slotID == null) return;
            uiItems[slotID].SetSelectable(isSelectable);
        }

        public void RemoveItem(int? slotID)
        {
            if(slotID == null) return;
            uiItems[slotID].Clear();
        }
        
        private void OnEnterItem(int? slotID) => OnEnter?.Invoke(slotID);
        private void OnExitItem() => OnExit?.Invoke();

        public void ShowTooltip()
        {
            
        }

        public void HideTooltip()
        {
            
        }

        private void OnDestroy()
        {
            for (int i = 0; i < uiItems.Count; i++)
            {
                uiItems[i].OnClickedLeftMouse -= OnLeftMouseClickItem;
                uiItems[i].OnClickedRightMouse -= OnRightMouseClickItem;
                uiItems[i].OnEnter -= OnEnter;
                uiItems[i].OnExit -= OnExit;
            }
        }
    }
}