using System;
using ScriptableObjects.Gameplay;
using Unit;
using Unit.Character;
using UnityEngine;
using Zenject;

namespace Gameplay.Units.Item.Loot
{
    public class LootInteractionHandler : IInteractionHandler
    {
        [Inject] private SO_GameHotkeys so_GameHotkeys;
        
        public ILoot CurrentLoot { get; private set; }

        private KeyCode takeLootKey;
        private readonly GameObject gameObject;
        private readonly CharacterControlDesktop characterControlDesktop;
        
        public LootInteractionHandler(GameObject gameObject, CharacterControlDesktop characterControlDesktop)
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
            takeLootKey = so_GameHotkeys.TakeLootKey;
        }

        public void SetLoot(ILoot loot)
        {
            CurrentLoot = loot;
        }

        public void HandleInput()
        {
            if(!Input.GetKeyDown(takeLootKey)) return;
            
            if(CurrentLoot == null) return;

            CurrentLoot.TakeLoot(gameObject);
            var colliders = Physics.OverlapSphere(gameObject.transform.position, 0.5f, Layers.LOOT_LAYER);
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out ILoot loot) && 
                    loot != CurrentLoot)
                {
                    SetLoot(loot);
                    CurrentLoot.Enable(takeLootKey);
                    return;
                }
            }

            SetLoot(null);
            characterControlDesktop.ClearHotkeys();
        }

        public void CheckTriggerEnter(GameObject other)
        {
            if (other.TryGetComponent(out ILoot loot))
            {
                loot.Enable(takeLootKey);
                SetLoot(loot);
            }
        }

        public void CheckTriggerExit(GameObject other)
        {
            if (other.TryGetComponent(out ILoot loot))
            {
                loot.Disable();
                SetLoot(null);
            }
        }
        
    }
}