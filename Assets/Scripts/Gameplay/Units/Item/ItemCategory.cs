using System;

[Flags]
public enum ItemCategory
{
    Nothing = 0,
    Equipment = 1 << 0,
    Meat = 1 << 1,
    Plant = 1 << 2,
}

public enum ItemBehaviour
{
    Nothing = 0,
    NoTarget,
    UnitTarget,
    PointTarget,
    Passive,
}

public enum ItemUsageType
{
    Nothing,
    Drink,
    Throw,
    ApplyToSelf,
    ApplyToTarget,
    ApplyToPoint,
    Equip,
}