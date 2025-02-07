using System;
using ScriptableObjects.Gameplay;
using Unit.Character;
using UnityEngine;
using Zenject;

namespace Unit.Item.Container
{
    public class ContainerInteractionHandler : IInteractionHandler
    {
        [Inject] private SO_GameHotkeys so_GameHotkeys;
        
        public IContainer CurrentContainer { get; private set; }
        
        private KeyCode openContainerKey;
        private readonly GameObject gameObject;
        private readonly CharacterControlDesktop characterControlDesktop;

        public ContainerInteractionHandler(GameObject gameObject, CharacterControlDesktop characterControlDesktop)
        {
            this.gameObject = gameObject;
            this.characterControlDesktop = characterControlDesktop;
        }

        public bool IsBlocked()
        {
            return true;
        }
        
        public void Initialize()
        {
            openContainerKey = so_GameHotkeys.OpenContainerKey;
        }
        
        public void SetContainer(IContainer container)
        {
            CurrentContainer = container;
        }

        public void HandleInput()
        {
            if(!Input.GetKeyDown(openContainerKey)) return;
            
            if (CurrentContainer == null) return;
            
            CurrentContainer.Open();
            var colliders = Physics.OverlapSphere(gameObject.transform.position, 0.5f);
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out IContainer container))
                {
                    SetContainer(container);
                    CurrentContainer.Enable(openContainerKey);
                    return;
                }
            }

            SetContainer(null);
            characterControlDesktop.ClearHotkeys();
        }

        public void CheckTriggerEnter(GameObject other)
        {
            if (other.TryGetComponent(out IContainer container))
            {
                container.Enable(openContainerKey);
                SetContainer(container);
            }
        }
        
        public void CheckTriggerExit(GameObject other)
        {
            if (other.TryGetComponent(out IContainer container))
            {
                container.Disable();
                SetContainer(null);
            }
        }
    }
}