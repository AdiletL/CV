using System;

[Flags]
public enum InputType
{
    Nothing = 0,
    Everything = ~0,
    Movement = 1 << 0,
    Jump = 1 << 1,
    SelectObject = 1 << 2,
    Attack = 1 << 3,
    SpecialAction = 1 << 4,
    Item = 1 << 5,
    Ability = 1 << 6,
}