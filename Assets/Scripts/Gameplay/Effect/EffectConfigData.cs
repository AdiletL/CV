using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.Effect
{
    [System.Serializable]
    public class EffectConfigData
    {
        public List<EffectType> EffectTypesID;
        
        [ShowIf("@EffectTypesID.Contains(EffectType.SlowMovement)"), Space]
        public SlowMovementConfig SlowMovementConfig;
        
        [ShowIf("@EffectTypesID.Contains(EffectType.Vampirism)"), Space]
        public VampirismConfig VampirismConfig;
        
        [ShowIf("@EffectTypesID.Contains(EffectType.Disable)"), Space]
        public DisableConfig[] DisableConfigs;
    }
}