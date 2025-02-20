using Gameplay.Ability;
using UnityEngine;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIAbilityInventory : UIInventory
    {
        [SerializeField] private UIAbilityContainer abilityContainer;

        public override void SetMaxCountCells(int value)
        {
            abilityContainer.CreateCells(value);
        }
        public void AddAbility(int? slotID, Sprite icon, bool isSelectable, float currentCooldown, float maxCooldown) => abilityContainer.AddAbility(slotID, icon, isSelectable, currentCooldown, maxCooldown);
        public void UpdateCooldown(int? slotID, float current, float max) => abilityContainer.UpdateCooldown(slotID, current, max);
        public void UpdateSelectable(int? slotID, bool isReady) => abilityContainer.ChangeSelectable(slotID, isReady);
        public void RemoveAbility(int? slotID) => abilityContainer.RemoveAbility(slotID);
    }
}