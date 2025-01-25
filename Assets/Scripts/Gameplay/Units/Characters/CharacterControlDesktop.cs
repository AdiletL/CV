using UnityEngine;

namespace Unit.Character
{
    public class CharacterControlDesktop : UnitControlDesktop
    {
        public override void Initialize()
        {
            
        }


        public virtual void ClearHotkeys()
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