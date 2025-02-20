using System.Collections.Generic;
using Gameplay.Ability;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIAbilityContainer : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject uiAbilityPrefab;
        [SerializeField] private Transform container;
        
        private Dictionary<int?, UIAbility> uiAbilities = new ();
        
        public void CreateCells(int maxCountItem)
        {
            UIAbility uiAbility = null;
            for (int i = 0; i < maxCountItem; i++)
            {
                var handle = Addressables.InstantiateAsync(uiAbilityPrefab, container).WaitForCompletion();
                uiAbility = handle.GetComponent<UIAbility>();
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
            uiAbilities[slotID].UpdateCooldownBar(currentCooldown, maxCooldown);
            uiAbilities[slotID].UpdateSelectable(isSelectable);
        }

        public void UpdateCooldown(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiAbilities[slotID].UpdateCooldownBar(current, max);
        }

        public void ChangeSelectable(int? slotID, bool isSelectable)
        {
            if(slotID == null) return;
            uiAbilities[slotID].UpdateSelectable(isSelectable);
        }

        public void RemoveAbility(int? slotID)
        {
            if(slotID == null) return;
            uiAbilities[slotID].Clear();
        }
    }
}