using Gameplay.UI.ScreenSpace.Stats;
using Zenject;

namespace Gameplay
{
    public class ShowStatsOnUI
    {
        private UIStats uiStats;
        private IStatsController statsController;
        
        public ShowStatsOnUI(IStatsController statsController, UIStats uiStats)
        {
            this.statsController = statsController;
            this.uiStats = uiStats;
            SubscribeEvent();
        }

        ~ShowStatsOnUI()
        {
            UnsubscribeEvent();
        }

        public void Initialize()
        {
            var damageStat = statsController.GetStat(StatType.Damage);
            if(damageStat != null) uiStats.SetDamage(damageStat.CurrentValue);

            var movementStat = statsController.GetStat(StatType.MovementSpeed);
            if(movementStat != null) uiStats.SetMovement(movementStat.CurrentValue);

            var armorStat = statsController.GetStat(StatType.Armor);
            if(armorStat != null) uiStats.SetArmor(armorStat.CurrentValue);

            var attackRangeStat = statsController.GetStat(StatType.AttackRange);
            if(attackRangeStat != null) uiStats.SetRange(attackRangeStat.CurrentValue);
           
            var healthStat = statsController.GetStat(StatType.Health);
            if(healthStat != null) uiStats.SetHealth(healthStat.CurrentValue, healthStat.MaximumValue);
            
            var enduranceStat = statsController.GetStat(StatType.Endurance);
            if(enduranceStat != null) uiStats.SetEndurance(enduranceStat.CurrentValue, enduranceStat.MaximumValue);
            
            var manaStat = statsController.GetStat(StatType.Mana);
            if(manaStat != null) uiStats.SetMana(manaStat.CurrentValue, manaStat.MaximumValue);
            
            var regenerationHealthStat = statsController.GetStat(StatType.RegenerationHealth);
            if(regenerationHealthStat != null) uiStats.SetRegenerationHealth(regenerationHealthStat.CurrentValue);
            
            var regenerationManaStat = statsController.GetStat(StatType.RegenerationMana);
            if(regenerationManaStat != null) uiStats.SetRegenerationMana(regenerationManaStat.CurrentValue);
            
            var regenerationEnduranceStat = statsController.GetStat(StatType.RegenerationEndurance);
            if(regenerationEnduranceStat != null) uiStats.SetRegenerationEndurance(regenerationEnduranceStat.CurrentValue);
        }

        private void SubscribeEvent()
        {
            var damageStat = statsController.GetStat(StatType.Damage);
            if (damageStat != null)
            {
                damageStat.OnAddCurrentValue += uiStats.SetDamage;
                damageStat.OnRemoveCurrentValue += uiStats.SetDamage;
            }
            
            var movementStat = statsController.GetStat(StatType.MovementSpeed);
            if (movementStat != null)
            {
                movementStat.OnAddCurrentValue += uiStats.SetMovement;
                movementStat.OnRemoveCurrentValue += uiStats.SetMovement;
            }
            
            var armorStat = statsController.GetStat(StatType.Armor);
            if (armorStat != null)
            {
                armorStat.OnAddCurrentValue += uiStats.SetArmor;
                armorStat.OnRemoveCurrentValue += uiStats.SetArmor;
            }
            
            var healthStat = statsController.GetStat(StatType.Health);
            if (healthStat != null)
            {
                healthStat.OnAddCurrentValue += uiStats.SetArmor;
                healthStat.OnRemoveCurrentValue += uiStats.SetArmor;
            }
            
            var enduranceStat = statsController.GetStat(StatType.Endurance);
            if (enduranceStat != null)
            {
                enduranceStat.OnAddCurrentValue += uiStats.SetArmor;
                enduranceStat.OnRemoveCurrentValue += uiStats.SetArmor;
            }
            
            var manaStat = statsController.GetStat(StatType.Mana);
            if (manaStat != null)
            {
                manaStat.OnAddCurrentValue += uiStats.SetArmor;
                manaStat.OnRemoveCurrentValue += uiStats.SetArmor;
            }
            
            var attackRangeStat = statsController.GetStat(StatType.AttackRange);
            if (attackRangeStat != null)
            {
                attackRangeStat.OnAddCurrentValue += uiStats.SetRange;
                attackRangeStat.OnRemoveCurrentValue += uiStats.SetRange;
            }
            
            var regenerationHealthStat = statsController.GetStat(StatType.RegenerationHealth);
            if (regenerationHealthStat != null)
            {
                regenerationHealthStat.OnAddCurrentValue += uiStats.SetRegenerationHealth;
                regenerationHealthStat.OnRemoveCurrentValue += uiStats.SetRegenerationHealth;
            }
            
            var regenerationManaStat = statsController.GetStat(StatType.RegenerationMana);
            if (regenerationManaStat != null)
            {
                regenerationManaStat.OnAddCurrentValue += uiStats.SetRegenerationMana;
                regenerationManaStat.OnRemoveCurrentValue += uiStats.SetRegenerationMana;
            }
            
            var regenerationEnduranceStat = statsController.GetStat(StatType.RegenerationEndurance);
            if (regenerationEnduranceStat != null)
            {
                regenerationEnduranceStat.OnAddCurrentValue += uiStats.SetRegenerationEndurance;
                regenerationEnduranceStat.OnRemoveCurrentValue += uiStats.SetRegenerationEndurance;
            }
        }

        private void UnsubscribeEvent()
        {
            var damageStat = statsController.GetStat(StatType.Damage);
            if (damageStat != null)
            {
                damageStat.OnAddCurrentValue -= uiStats.SetDamage;
                damageStat.OnRemoveCurrentValue -= uiStats.SetDamage;
            }
            
            var movementStat = statsController.GetStat(StatType.MovementSpeed);
            if (movementStat != null)
            {
                movementStat.OnAddCurrentValue -= uiStats.SetMovement;
                movementStat.OnRemoveCurrentValue -= uiStats.SetMovement;
            }
            
            var armorStat = statsController.GetStat(StatType.Armor);
            if (armorStat != null)
            {
                armorStat.OnAddCurrentValue -= uiStats.SetArmor;
                armorStat.OnRemoveCurrentValue -= uiStats.SetArmor;
            }
            
            var healthStat = statsController.GetStat(StatType.Health);
            if (healthStat != null)
            {
                healthStat.OnAddCurrentValue -= uiStats.SetArmor;
                healthStat.OnRemoveCurrentValue -= uiStats.SetArmor;
            }
            
            var enduranceStat = statsController.GetStat(StatType.Endurance);
            if (enduranceStat != null)
            {
                enduranceStat.OnAddCurrentValue -= uiStats.SetArmor;
                enduranceStat.OnRemoveCurrentValue -= uiStats.SetArmor;
            }
            
            var manaStat = statsController.GetStat(StatType.Mana);
            if (manaStat != null)
            {
                manaStat.OnAddCurrentValue -= uiStats.SetArmor;
                manaStat.OnRemoveCurrentValue -= uiStats.SetArmor;
            }
            
            var attackRangeStat = statsController.GetStat(StatType.AttackRange);
            if (attackRangeStat != null)
            {
                attackRangeStat.OnAddCurrentValue -= uiStats.SetRange;
                attackRangeStat.OnRemoveCurrentValue -= uiStats.SetRange;
            }
            
            var regenerationHealthStat = statsController.GetStat(StatType.RegenerationHealth);
            if (regenerationHealthStat != null)
            {
                regenerationHealthStat.OnAddCurrentValue -= uiStats.SetRegenerationHealth;
                regenerationHealthStat.OnRemoveCurrentValue -= uiStats.SetRegenerationHealth;
            }
            
            var regenerationManaStat = statsController.GetStat(StatType.RegenerationMana);
            if (regenerationManaStat != null)
            {
                regenerationManaStat.OnAddCurrentValue -= uiStats.SetRegenerationMana;
                regenerationManaStat.OnRemoveCurrentValue -= uiStats.SetRegenerationMana;
            }
            
            var regenerationEnduranceStat = statsController.GetStat(StatType.RegenerationEndurance);
            if (regenerationEnduranceStat != null)
            {
                regenerationEnduranceStat.OnAddCurrentValue -= uiStats.SetRegenerationEndurance;
                regenerationEnduranceStat.OnRemoveCurrentValue -= uiStats.SetRegenerationEndurance;
            }
        }
    }
}