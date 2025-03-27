using UnityEngine;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterControlDesktop
    {
        protected GameObject gameObject;
        protected bool isCanControl = true;
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        
        public virtual void Initialize()
        {
            
        }


        public virtual void ClearHotkeys()
        {
            
        }
        public virtual void HandleHotkey()
        {
            
        }

        public virtual void HandleInput()
        {
        }

        public virtual void ActivateControl()
        {
            isCanControl = true;
        }

        public virtual void DeactivateControl()
        {
            isCanControl = false;
        }
    }

    public abstract class CharacterControlDesktopBuilder
    {
        protected CharacterControlDesktop characterControlDesktop;
        public CharacterControlDesktopBuilder(CharacterControlDesktop instance)
        {
            characterControlDesktop = instance;
        }

        public CharacterControlDesktopBuilder SetGameObject(GameObject gameObject)
        {
            characterControlDesktop.SetGameObject(gameObject);
            return this;
        }
        
        public CharacterControlDesktop Build()
        {
            return characterControlDesktop;
        }
    }
}