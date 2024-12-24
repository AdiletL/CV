using System;
using UnityEngine;

namespace Unit
{
    public abstract class UnitControlDesktop : IControl
    {
        public abstract void Initialize();

        public abstract void HandleInput();
    }
}