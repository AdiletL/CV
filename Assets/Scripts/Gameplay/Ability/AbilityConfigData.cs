using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.Ability
{
    [System.Serializable]
    public class AbilityConfigData
    {
        public AbilityType AbilityTypeID;
        
        [ShowIf("@AbilityTypeID.HasFlag(AbilityType.ApplyDamageHeal)"), Space]
        public ApplyDamageHealConfig ApplyDamageHealConfig;
        
        [ShowIf("@AbilityTypeID.HasFlag(AbilityType.SpawnPortal)"), Space]
        public SpawnPortalConfig SpawnPortalConfig;
        
        [ShowIf("@AbilityTypeID.HasFlag(AbilityType.BlockPhysicalDamage)"), Space]
        public BlockPhysicalDamageConfig BlockPhysicalDamageConfig;
    }
}