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

        public void AddAbility(AbilityData data)
        {
            if(data.SlotID == null) return;
            
            uiAbilities[data.SlotID].SetSlotID(data.SlotID);
            uiAbilities[data.SlotID].SetIcon(data.Icon);
            uiAbilities[data.SlotID].UpdateCooldownBar(0, 1);
            if ((data.AbilityBehaviour & AbilityBehaviour.Passive) == 0)
                uiAbilities[data.SlotID].ChangeReadiness(true);
            else
                uiAbilities[data.SlotID].ChangeReadiness(false);
        }

        public void UpdateCooldown(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiAbilities[slotID].UpdateCooldownBar(current, max);
        }

        public void ChangeReadiness(int? slotID, bool isReady)
        {
            if(slotID == null) return;
            uiAbilities[slotID].ChangeReadiness(isReady);
        }

        public void RemoveAbility(int? slotID)
        {
            if(slotID == null) return;
            uiAbilities[slotID].Clear();
        }
    }
}