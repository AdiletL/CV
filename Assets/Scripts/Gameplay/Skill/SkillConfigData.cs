using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.Skill
{
    [System.Serializable]
    public class SkillConfigData
    {
        public SkillType SkillTypeID;
        
        [ShowIf("@SkillTypeID.HasFlag(SkillType.applyDamageHeal)"), Space]
        public ApplyDamageHealConfig ApplyDamageHealConfig;
        
        [ShowIf("@SkillTypeID.HasFlag(SkillType.spawnPortal)"), Space]
        public SpawnPortalConfig SpawnPortalConfig;
        
        [ShowIf("@SkillTypeID.HasFlag(SkillType.blockPhysicalDamage)"), Space]
        public BlockPhysicalDamageConfig BlockPhysicalDamageConfig;
    }
}