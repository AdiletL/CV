using System.Collections;
using Gameplay.UI;
using ScriptableObjects.Unit.Item.Container;
using UnityEngine;

namespace Gameplay.Unit.Container
{
    public class ChestController : ContainerController
    {
        private SO_Chest so_Chest;
        private HotkeyUI hotkeyUI;
        private ChestAnimation chestAnimation;
        
        private bool isInteractable;

        public override void Initialize()
        {
            base.Initialize();

            so_Chest = (SO_Chest)so_Container;
            chestAnimation = (ChestAnimation)containerAnimation;
            hotkeyUI = GetComponentInUnit<HotkeyUI>();
            hotkeyUI.Hide();
        }

        public override void Appear()
        {
            
        }

        public override void Disappear()
        {
            throw new System.NotImplementedException();
        }

        public override async void Open()
        {
            if(isOpened) return;
            isOpened = true;
            chestAnimation.ChangeAnimationWithSpeed(openClip);
            Disable();
            await SpawnItems();
        }

        public override void Close()
        {
            chestAnimation.ChangeAnimationWithSpeed(closeClip);
            isOpened = false;
        }

        public override void Enable(KeyCode openKey)
        {
            hotkeyUI.SetText(openKey.ToString());
            if(isOpened) return;
            
            hotkeyUI.Show();
        }

        public override void Disable()
        {
            hotkeyUI.Hide();
        }
    }
}
