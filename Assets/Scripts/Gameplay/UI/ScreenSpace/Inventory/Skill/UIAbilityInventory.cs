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
        public void AddAbility(AbilityData data) => abilityContainer.AddAbility(data);
        public void UpdateCooldown(int? slotID, float current, float max) => abilityContainer.UpdateCooldown(slotID, current, max);
        public void ChangeReadiness(int? slotID, bool isReady) => abilityContainer.ChangeReadiness(slotID, isReady);
        public void RemoveAbility(int? slotID) => abilityContainer.RemoveAbility(slotID);
    }
}