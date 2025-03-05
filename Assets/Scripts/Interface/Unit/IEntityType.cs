using System;

[Flags]
public enum EntityType
{
    Nothing = 0,
    Human = 1 << 0,
    Monster = 1 << 1,
    Weapon = 1 << 2,
    Plant = 1 << 3,
}