using Gameplay.UI.ScreenSpace.Stats;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class MediatorStatsAndUI
    {
        private UIStats uiStats;
        private IStatsController<UnitStatType> statsController;

        private Stat levelStat;
        private Stat experienceStat;
        private Stat damageStat;
        private Stat movementStat;
        private Stat armorStat;
        private Stat rangeAttackStat;
        private Stat healthStat;
        private Stat enduranceStat;
        private Stat manaStat;
        private Stat regenerationHealthStat;
        private Stat regenerationEnduranceStat;
        private Stat regenerationManaStat;
        private Stat evasionStat;
        
        public MediatorStatsAndUI(IStatsController<UnitStatType> statsController, UIStats uiStats)
        {
            this.statsController = statsController;
            this.uiStats = uiStats;
        }

        ~MediatorStatsAndUI()
        {
            UnsubscribeEvent();
        }

        public void Initialize()
        {
            levelStat = statsController.GetStat(UnitStatType.Level);
            if(levelStat != null) uiStats.SetLevel(levelStat.CurrentValue);
            
            experienceStat = statsController.GetStat(UnitStatType.Experience);
            if(experienceStat != null) uiStats.SetExperience(experienceStat.CurrentValue, experienceStat.MaximumValue);
            
            damageStat = statsController.GetStat(UnitStatType.Damage);
            if(damageStat != null) uiStats.SetDamage(damageStat.CurrentValue);

            movementStat = statsController.GetStat(UnitStatType.MovementSpeed);
            if(movementStat != null) uiStats.SetMovementSpeed(movementStat.CurrentValue);

            armorStat = statsController.GetStat(UnitStatType.PhysicalDamageResistance);
            if(armorStat != null) uiStats.SetArmor(armorStat.CurrentValue);

            rangeAttackStat = statsController.GetStat(UnitStatType.AttackRange);
            if(rangeAttackStat != null) uiStats.SetRangeAttack(rangeAttackStat.CurrentValue);
           
            healthStat = statsController.GetStat(UnitStatType.Health);
            if(healthStat != null) uiStats.SetHealth(healthStat.CurrentValue, healthStat.MaximumValue);
            
            enduranceStat = statsController.GetStat(UnitStatType.Endurance);
            if(enduranceStat != null) uiStats.SetEndurance(enduranceStat.CurrentValue, enduranceStat.MaximumValue);
            
            manaStat = statsController.GetStat(UnitStatType.Mana);
            if(manaStat != null) uiStats.SetMana(manaStat.CurrentValue, manaStat.MaximumValue);
            
            regenerationHealthStat = statsController.GetStat(UnitStatType.RegenerationHealth);
            if(regenerationHealthStat != null) uiStats.SetRegenerationHealth(regenerationHealthStat.CurrentValue);
            
            regenerationManaStat = statsController.GetStat(UnitStatType.RegenerationMana);
            if(regenerationManaStat != null) uiStats.SetRegenerationMana(regenerationManaStat.CurrentValue);
            
            regenerationEnduranceStat = statsController.GetStat(UnitStatType.RegenerationEndurance);
            if(regenerationEnduranceStat != null) uiStats.SetRegenerationEndurance(regenerationEnduranceStat.CurrentValue);
            
            evasionStat = statsController.GetStat(UnitStatType.Evasion);
            if(evasionStat != null) uiStats.SetEvasion(evasionStat.CurrentValue * 100);
            SubscribeEvent();
        }

        private void SubscribeEvent()
        {
            if (levelStat != null) levelStat.OnChangedCurrentValue += OnChangedLevelStatCurrentValue;
            if (experienceStat != null)
            {
                experienceStat.OnChangedCurrentValue += OnChangedExperienceStatCurrentValue;
                experienceStat.OnChangedMaximumValue += OnChangedExperienceStatMaximumValue;
            }
            if (damageStat != null) damageStat.OnChangedCurrentValue += OnChangedDamageStatCurrentValue;
            if (movementStat != null) movementStat.OnChangedCurrentValue += OnChangedMovementStatCurrentValue;
            if (armorStat != null) armorStat.OnChangedCurrentValue += OnChangedArmorStatCurrentValue;
            if (rangeAttackStat != null) rangeAttackStat.OnChangedCurrentValue += OnChangedRangeAttackStatCurrentValue;
            if (healthStat != null) healthStat.OnChangedCurrentValue += OnChangedHealthStatCurrentValue;
            if (enduranceStat != null) enduranceStat.OnChangedCurrentValue += OnChangedEnduranceStatCurrentValue;
            if (manaStat != null) manaStat.OnChangedCurrentValue += OnChangedManaStatCurrentValue;
            if (regenerationHealthStat != null) regenerationHealthStat.OnChangedCurrentValue += OnChangedRegenerationHealthStatCurrentValue;
            if (regenerationEnduranceStat != null) regenerationEnduranceStat.OnChangedCurrentValue += OnChangedRegenerationEnduranceStatCurrentValue;
            if (regenerationManaStat != null) regenerationManaStat.OnChangedCurrentValue += OnChangedRegenerationManaStatCurrentValue;
            if (evasionStat != null) evasionStat.OnChangedCurrentValue += OnChangedEvasionStatCurrentValue;
        }

        private void UnsubscribeEvent()
        {
            if (levelStat != null) levelStat.OnChangedCurrentValue -= OnChangedLevelStatCurrentValue;
            if (experienceStat != null)
            {
                experienceStat.OnChangedCurrentValue -= OnChangedExperienceStatCurrentValue;
                experienceStat.OnChangedMaximumValue -= OnChangedExperienceStatMaximumValue;
            }
            if (damageStat != null) damageStat.OnChangedCurrentValue -= OnChangedDamageStatCurrentValue;
            if (movementStat != null) movementStat.OnChangedCurrentValue -= OnChangedMovementStatCurrentValue;
            if (armorStat != null) armorStat.OnChangedCurrentValue -= OnChangedArmorStatCurrentValue;
            if (rangeAttackStat != null) rangeAttackStat.OnChangedCurrentValue -= OnChangedRangeAttackStatCurrentValue;
            if (healthStat != null) healthStat.OnChangedCurrentValue -= OnChangedHealthStatCurrentValue;
            if (enduranceStat != null) enduranceStat.OnChangedCurrentValue -= OnChangedEnduranceStatCurrentValue;
            if (manaStat != null) manaStat.OnChangedCurrentValue -= OnChangedManaStatCurrentValue;
            if (regenerationHealthStat != null) regenerationHealthStat.OnChangedCurrentValue -= OnChangedRegenerationHealthStatCurrentValue;
            if (regenerationEnduranceStat != null) regenerationEnduranceStat.OnChangedCurrentValue -= OnChangedRegenerationEnduranceStatCurrentValue;
            if (regenerationManaStat != null) regenerationManaStat.OnChangedCurrentValue -= OnChangedRegenerationManaStatCurrentValue;
        }
        
        private void OnChangedLevelStatCurrentValue() => uiStats.SetLevel(levelStat.CurrentValue);
        private void OnChangedExperienceStatCurrentValue() => uiStats.SetExperience(experienceStat.CurrentValue, experienceStat.MaximumValue);
        private void OnChangedExperienceStatMaximumValue() => uiStats.SetExperience(experienceStat.CurrentValue, experienceStat.MaximumValue);
        private void OnChangedDamageStatCurrentValue() => uiStats.SetDamage(damageStat.CurrentValue);
        private void OnChangedMovementStatCurrentValue() => uiStats.SetMovementSpeed(movementStat.CurrentValue);
        private void OnChangedArmorStatCurrentValue() => uiStats.SetArmor(armorStat.CurrentValue);
        private void OnChangedRangeAttackStatCurrentValue() => uiStats.SetRangeAttack(rangeAttackStat.CurrentValue);
        private void OnChangedHealthStatCurrentValue() => uiStats.SetHealth(healthStat.CurrentValue, healthStat.MaximumValue);
        private void OnChangedEnduranceStatCurrentValue() => uiStats.SetEndurance(enduranceStat.CurrentValue, enduranceStat.MaximumValue);
        private void OnChangedManaStatCurrentValue() => uiStats.SetMana(manaStat.CurrentValue, manaStat.MaximumValue);
        private void OnChangedRegenerationHealthStatCurrentValue() => uiStats.SetRegenerationHealth(regenerationHealthStat.CurrentValue);
        private void OnChangedRegenerationEnduranceStatCurrentValue() => uiStats.SetRegenerationEndurance(regenerationEnduranceStat.CurrentValue);
        private void OnChangedRegenerationManaStatCurrentValue() => uiStats.SetRegenerationMana(regenerationManaStat.CurrentValue);
        private void OnChangedEvasionStatCurrentValue() => uiStats.SetEvasion(evasionStat.CurrentValue * 100);
    }
}