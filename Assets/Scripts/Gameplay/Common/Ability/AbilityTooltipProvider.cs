using System.Collections.Generic;
using System.Globalization;
using Gameplay.UI.ScreenSpace;
using ScriptableObjects.Ability;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Gameplay.Ability
{
    public class AbilityTooltipProvider
    {
        [Inject] private UITooltipView uiTooltipView;
        [Inject] private SO_AbilityContainer so_AbilityContainer;
         

        public AbilityTooltipProvider()
        {
            
        }

        public void ShowTooltip(Ability ability)
        {
            var abilityConfig = so_AbilityContainer.GetAbilityConfig(ability.AbilityTypeID);

            var header = abilityConfig.DescriptionConfig.GetHeader();
            if(!string.IsNullOrEmpty(header))
                uiTooltipView.SetHeader(abilityConfig.DescriptionConfig.GetHeader());
            
            var description = abilityConfig.DescriptionConfig.GetDescription();
            if(!string.IsNullOrEmpty(description))
                uiTooltipView.SetDescription(description);

            List<string> stats = new List<string>(abilityConfig.AbilityStatConfigData.AbilityStatConfigs.Length);
            foreach (var abilityStatConfig in abilityConfig.AbilityStatConfigData.AbilityStatConfigs)
            {
                foreach (var statValueConfig in abilityStatConfig.StatValuesConfig)
                {
                    var statValue = AbilityStatConvertToString.Convert(
                        abilityStatConfig.AbilityStatTypeID, 
                        ability.GetStat(abilityStatConfig.AbilityStatTypeID).GetValue(statValueConfig.StatValueTypeID), 
                        statValueConfig.GameValueConfig.ValueTypeID, 
                        abilityConfig.MaxLevelScale, 
                        statValueConfig.ScalingValue);
                    
                    if(!string.IsNullOrEmpty(statValue))
                        stats.Add(statValue);
                }
            }

            foreach (var unitStatConfig in abilityConfig.UnitStatConfigData.StatConfigs)
            {
                foreach (var statValueConfig in unitStatConfig.StatValuesConfig)
                {
                    var statValue = UnitStatConvertToString.Convert(unitStatConfig.UnitStatTypeID,
                        ability.GetStat(unitStatConfig.UnitStatTypeID).GetValue(statValueConfig.StatValueTypeID),
                        statValueConfig.GameValueConfig.ValueTypeID,
                        abilityConfig.MaxLevelScale, 
                        statValueConfig.ScalingValue);
                    
                    if (!string.IsNullOrEmpty(statValue))
                        stats.Add(statValue);
                }
            }
            
            if(stats.Count != 0)
                uiTooltipView.SetStats(stats);
            
            uiTooltipView.Show();
        }

        public void HideTooltip()
        {
            uiTooltipView.Hide();
            uiTooltipView.Clear();
        }
    }
}