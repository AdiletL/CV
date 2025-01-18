using System;
using UnityEngine;

namespace Unit
{
    public abstract class UnitControlDesktop : IControl
    {
        public abstract void Initialize();

        public abstract void HandleHotkey();
        public abstract void HandleInput();
    }

    public class UnitControlDesktopBuilder<T> where T : UnitControlDesktop
    {
        protected UnitControlDesktop unitControlDesktop;

        public UnitControlDesktopBuilder(UnitControlDesktop instance)
        {
            unitControlDesktop = instance;
        }

        public UnitControlDesktop Build()
        {
            return unitControlDesktop;
        }
    }
}