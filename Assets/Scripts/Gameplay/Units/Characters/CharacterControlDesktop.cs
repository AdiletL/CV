using UnityEngine;

namespace Unit.Character
{
    public class CharacterControlDesktop : UnitControlDesktop
    {
        public override void Initialize()
        {
            
        }


        protected virtual void ClearHotkey()
        {
            
        }
        public override void HandleHotkey()
        {
            
        }

        public override void HandleInput()
        {
        }
    }

    public class CharacterControlDesktopBuilder : UnitControlDesktopBuilder<CharacterControlDesktop>
    {
        public CharacterControlDesktopBuilder(UnitControlDesktop instance) : base(instance)
        {
        }
    }
}