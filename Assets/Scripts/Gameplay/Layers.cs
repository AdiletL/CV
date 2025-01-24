
using UnityEngine;

public static class Layers
{
    public readonly static LayerMask EVERYTHING_LAYER = ~0;
    public readonly static LayerMask PLAYER_LAYER = 1 << 3;
    public readonly static LayerMask CREEP_LAYER = 1 << 7; 
    public readonly static LayerMask CELL_LAYER = 1 << 6;
    public readonly static LayerMask LOOT_LAYER = 1 << 8;
}
