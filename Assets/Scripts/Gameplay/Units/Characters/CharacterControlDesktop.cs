﻿using UnityEngine;

namespace Gameplay.Unit.Character
{
    public class CharacterControlDesktop : UnitControlDesktop
    {
        protected GameObject gameObject;
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        
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

        public CharacterControlDesktopBuilder SetGameObject(GameObject gameObject)
        {
            if(unitControlDesktop is CharacterControlDesktop characterControlDesktop)
                characterControlDesktop.SetGameObject(gameObject);
            
            return this;
        }
    }
}