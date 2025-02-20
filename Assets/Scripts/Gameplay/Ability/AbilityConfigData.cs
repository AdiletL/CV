using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.Ability
{
    [System.Serializable]
    public class AbilityConfigData
    {
        public List<AbilityType> AbilityTypeID;
        
        [ShowIf("@AbilityTypeID.Contains(AbilityType.ApplyDamageHeal)"), Space]
        public ApplyDamageHealConfig ApplyDamageHealConfig;
        
        [ShowIf("@AbilityTypeID.Contains(AbilityType.BlockPhysicalDamage)"), Space]
        public BlockPhysicalDamageConfig BlockPhysicalDamageConfig;
    }
}