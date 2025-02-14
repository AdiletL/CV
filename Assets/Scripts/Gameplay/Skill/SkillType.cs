using System;

[Flags]
public enum SkillType
{
    nothing,
    dash = 1 << 0,
    spawnPortal = 1 << 1,
    applyDamageHeal = 1 << 2,
    blockPhysicalDamage = 1 << 3,
    blockMagicDamage = 1 << 4,
}