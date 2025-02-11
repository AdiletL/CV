using System;
using System.Collections.Generic;
using ScriptableObjects.Gameplay;
using Unit;
using Unit.Character;
using Unit.Item;
using UnityEngine;
using Zenject;

namespace Gameplay.Units.Item.Loot
{
    public class ItemInteractionHandler : IInteractionHandler
    {
        [Inject] private SO_GameHotkeys so_GameHotkeys;
        
        public IItem CurrentIItem { get; private set; }

        private float countCooldownCheckItems;
        private readonly float countCooldownCheck = .5f;
        
        private KeyCode takeLootKey;
        private readonly GameObject gameObject;
        private readonly CharacterControlDesktop characterControlDesktop;

        private List<IItem> currentItems = new(10);
        
        public ItemInteractionHandler(GameObject gameObject, CharacterControlDesktop characterControlDesktop)
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

        public void SetItem(IItem iItem)
        {
            CurrentIItem = iItem;
        }

        public void HandleInput()
        {
            CheckItems();
            
            if(!Input.GetKeyDown(takeLootKey)) return;
            if(CurrentIItem == null) return;

            CurrentIItem.TakeItem(gameObject);
            RemoveItem(CurrentIItem);

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
                if (CurrentIItem == null)
                {
                    for (int i = currentItems.Count - 1; i >= 0; i--)
                    {
                        var item = currentItems[i];
                        if (item.IsCanTake)
                        {
                            item.Enable(takeLootKey);
                            SetItem(item);
                            break;
                        }
                    }
                }
                countCooldownCheckItems = 0;
            }
        }

        private void AddItem(IItem item)
        {
            if(currentItems.Contains(item)) return;
            currentItems.Add(item);
        }

        private void RemoveItem(IItem item)
        {
            if(!currentItems.Contains(item)) return;
            item.Disable();
            currentItems.Remove(item);
            if(currentItems.Count == 0)
                SetItem(null);
        }

        public void CheckTriggerEnter(GameObject other)
        {
            if (other.TryGetComponent(out IItem item))
            {
                AddItem(item);
                if (!item.IsCanTake || CurrentIItem != null) return;
                item.Enable(takeLootKey);
                SetItem(item);
            }
        }

        public void CheckTriggerExit(GameObject other)
        {
            if (other.TryGetComponent(out IItem item))
            {
                RemoveItem(item);
                item.Disable();
            }
        }
    }
}