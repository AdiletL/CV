using System.Collections.Generic;
using Gameplay.Ability;
using Gameplay.UI.ScreenSpace;
using ScriptableObjects.Unit.Item;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Item
{
    public class ItemTooltipProvider
    {
        [Inject] private UITooltipView uiTooltipView;
        [Inject] private SO_ItemContainer so_ItemContainer;
         

        public ItemTooltipProvider()
        {
            
        }

        public void ShowTooltip(Item item)
        {
            var itemConfig = so_ItemContainer.GetItemConfig(item.ItemName);

            var header = itemConfig.DescriptionConfig.GetHeader();
            if(!string.IsNullOrEmpty(header))
                uiTooltipView.SetHeader(header);

            List<string> stats = new List<string>(itemConfig.StatsConfigData.StatConfigs.Length);
            foreach (var unitStatConfig in itemConfig.StatsConfigData.StatConfigs)
            {
                foreach (var statValueConfig in unitStatConfig.StatValuesConfig)
                {
                    var statValue = UnitStatConvertToString.Convert(unitStatConfig.UnitStatTypeID,
                        item.GetStat(unitStatConfig.UnitStatTypeID).GetValue(statValueConfig.StatValueTypeID),
                        statValueConfig.GameValueConfig.ValueTypeID,
                        scalingValue: statValueConfig.ScalingValue);
                    
                    if (!string.IsNullOrEmpty(statValue))
                        stats.Add(statValue);
                }
            }
            
            var description = itemConfig.DescriptionConfig.GetDescription();
            if (!string.IsNullOrEmpty(description))
                uiTooltipView.SetDescription(description);
            
            if(stats.Count > 0)
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