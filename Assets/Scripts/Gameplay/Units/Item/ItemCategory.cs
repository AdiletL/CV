using System;

public enum ItemName
{
    Nothing,
    MadnessMask,
    TeleportationScroll,
    NormalSword,
    NormalShield,
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
    Nothing = 0,
    NoTarget = 1 << 0,   // Моментальное применение (Blink)
    UnitTarget = 1 << 1, // Требует выбора цели (Hex)
    PointTarget = 1 << 2,// Требует точки на земле (Sun Strike)
    Toggle = 1 << 3,     // Включается/выключается (Armlet)
    Channeled = 1 << 4,  // Удерживаемый каст (Black Hole)
    Passive = 1 << 5,    // Пассивная способность (Critical Strike)
    Hidden = 1 << 6   // Скрытая способность (Invoke)
}