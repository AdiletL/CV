using System;
using Gameplay.UI.ScreenSpace.ContextMenu;
using UnityEngine;
using Zenject;

namespace Gameplay.Units.Item.ContextMenu
{
    public class EquipmentContextMenu
    {
        [Inject] private UIContextMenu uiContextMenu;

        private Item item;
        
        private readonly string[] headers = new string[]
        {
            PUT_ON, 
            TAKE_OFF
        };
        
        private const string PUT_ON = "Put on";
        private const string TAKE_OFF = "Take off";

        public EquipmentContextMenu(Item item)
        {
            this.item = item;
        }
        
        public void Show()
        {
            uiContextMenu.OnOptionSelected += OnOptionSelected;
            uiContextMenu.AddOptions(headers);
            uiContextMenu.transform.position = Input.mousePosition;
        }

        public void Hide()
        {
            uiContextMenu.HideOptions();
            uiContextMenu.OnOptionSelected -= OnOptionSelected;
        }

        private void OnOptionSelected(int slotID)
        {
            switch (slotID)
            {
                case 0: item.PutOn(); break;
                case 1: item.TakeOff(); break;
                default: throw new ArgumentException();
            }
            Hide();
        }
    }
}