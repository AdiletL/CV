﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIAbilityInventory : UIInventory
    {
        public event Action<int?> OnClickedLeftMouse;
        public event Action<int?> OnEnter;
        public event Action OnExit;
        
        [SerializeField] private AssetReferenceGameObject uiAbilityPrefab;
        [SerializeField] private Transform container;
        
        private Dictionary<int?, UIAbility> uiAbilities = new ();
        

        public override void CreateCells(int maxCountItem)
        {
            UIAbility uiAbility = null;
            for (int i = 0; i < maxCountItem; i++)
            {
                var handle = Addressables.InstantiateAsync(uiAbilityPrefab, container).WaitForCompletion();
                uiAbility = handle.GetComponent<UIAbility>();
                uiAbility.OnClickedLeftMouse += OnClickedLeftMouse;
                uiAbility.OnEnter += OnEnter;
                uiAbility.OnExit += OnExit;
                uiAbility.Initialize();
                uiAbility.Clear();
                uiAbilities.Add(i, uiAbility);
            }
        }
        
        public void AddAbility(int? slotID, Sprite icon, bool isSelectable, float currentCooldown, float maxCooldown)
        {
            if(slotID == null) return;
            uiAbilities[slotID].SetSlotID(slotID);
            uiAbilities[slotID].SetIcon(icon);
            uiAbilities[slotID].SetCooldownBar(currentCooldown, maxCooldown);
            uiAbilities[slotID].SetSelectable(isSelectable);
        }

        public void SetCooldown(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiAbilities[slotID].SetCooldownBar(current, max);
        }

        public void SetSelectable(int? slotID, bool isSelectable)
        {
            if(slotID == null) return;
            uiAbilities[slotID].SetSelectable(isSelectable);
        }

        public void RemoveAbility(int? slotID)
        {
            if(slotID == null) return;
            uiAbilities[slotID].Clear();
        }
        
        private void OnEnterItem(int? slotID) => OnEnter?.Invoke(slotID);
        private void OnExitItem() => OnExit?.Invoke();

        private void OnDestroy()
        {
            for (int i = 0; i < uiAbilities.Count; i++)
            {
                uiAbilities[i].OnClickedLeftMouse -= OnClickedLeftMouse;
            }
        }
    }
}