﻿using UnityEngine;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public abstract class UIInventory : MonoBehaviour
    {
        public abstract void CreateCells(int value);
    }
}