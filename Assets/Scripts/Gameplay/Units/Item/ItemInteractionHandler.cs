using System;
using System.Collections.Generic;
using Gameplay.Unit.Character;
using Gameplay.Unit.Item;
using ScriptableObjects.Gameplay;
using Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Item
{
    public class ItemInteractionHandler : IInteractionHandler
    {
        [Inject] private SO_GameHotkeys so_GameHotkeys;
        
        public ItemController CurrentItemController { get; private set; }

        private float countCooldownCheckItems;
        private readonly float countCooldownCheck = .5f;
        
        private KeyCode takeLootKey;
        private readonly GameObject gameObject;
        private readonly CharacterControlDesktop characterControlDesktop;

        private List<ItemController> currentItems = new(10);
        
        public ItemInteractionHandler(GameObject gameObject, CharacterControlDesktop characterControlDesktop)
        {
            this.gameObject = gameObject;
            this.characterControlDesktop = characterControlDesktop;
        }
        
        public void Initialize()
        {
            takeLootKey = so_GameHotkeys.TakeLootKey;
        }

        public void SetItem(ItemController itemController)
        {
            CurrentItemController = itemController;
        }

        public void HandleInput()
        {
            if (currentItems.Count > 0)
                CheckItems();
            else
                return;
            
            if(CurrentItemController == null) return;
            if(!Input.GetKeyDown(takeLootKey)) return;
            
            CurrentItemController.TakeItem(gameObject);
            RemoveItem(CurrentItemController);

            if (currentItems.Count != 0)
            {
                var item = currentItems[^1];
                item.Enable(takeLootKey);
                SetItem(item);
            }
            characterControlDesktop.ClearHotkeys();
        }

        private void CheckItems()
        {
            countCooldownCheckItems += Time.deltaTime;
            if (countCooldownCheckItems >= countCooldownCheck)
            {
                if (CurrentItemController == null)
                {
                    for (int i = currentItems.Count - 1; i >= 0; i--)
                    {
                        var item = currentItems[i];
                        if (item.IsCanTake)
                        {
                            CurrentItemController = item;
                            item.Enable(takeLootKey);
                            SetItem(item);
                            break;
                        }
                    }
                }
                countCooldownCheckItems = 0;
            }
        }

        private void AddItem(ItemController itemController)
        {
            if(currentItems.Contains(itemController)) return;
            currentItems.Add(itemController);
        }

        private void RemoveItem(ItemController itemController)
        {
            if(!currentItems.Contains(itemController)) return;
            itemController.Disable();
            currentItems.Remove(itemController);
            if(currentItems.Count == 0)
                SetItem(null);
        }

        public void CheckTriggerEnter(GameObject other)
        {
            if (other.TryGetComponent(out ItemController item))
            {
                AddItem(item);
                if (!item.IsCanTake || CurrentItemController != null) return;
                item.Enable(takeLootKey);
                SetItem(item);
            }
        }

        public void CheckTriggerExit(GameObject other)
        {
            if (other.TryGetComponent(out ItemController item))
            {
                RemoveItem(item);
            }
        }
    }
}