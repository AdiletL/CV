using System;

public enum ItemName
{
    Nothing,
    MadnessMask,
    TeleportationScroll,
    NormalSword,
}

[Flags]
public enum ItemCategory
{
    Nothing = 0,
    Weapon = 1 << 0,
    Meat = 1 << 1,
    Plant = 1 << 2,
}

public enum ItemBehaviour
{
    Nothing,
    Passive,
    Active,
    Toggle,
}