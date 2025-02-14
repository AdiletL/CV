using System;

[Flags]
public enum SkillType
{
    nothing,
    dash = 1 << 0,
    spawnTeleport = 1 << 1,
    applyDamageHeal = 1 << 2,
        
}