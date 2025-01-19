using System;
using Gameplay.UI;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.InteractableObject.Container;
using UnityEngine;
using Zenject;

namespace Unit.InteractableObject.Container
{
    public class ChestController : ContainerController
    {
        [Inject] private SO_GameHotkeys so_GameHotkeys;

        private SO_Chest so_chest;
        private HotkeyUI hotkeyUI;
        
        private bool isInteractable;

        private KeyCode openKey;
        
        public override void Initialize()
        {
            base.Initialize();

            so_chest = (SO_Chest)so_Container;
            openKey = so_GameHotkeys.OpenChestKey;

            hotkeyUI = GetComponentInUnit<HotkeyUI>();
            hotkeyUI.SetText(openKey.ToString());
            hotkeyUI.Hide();
        }

        public override void Appear()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(openKey) && 
                isInteractable &&
                !isOpened)
            {
                Open();
            }
        }
        
        public override void Open()
        {
            isOpened = true;
            GetComponentInUnit<ChestAnimation>().ChangeAnimationWithSpeed(openClip);
            Disable();
        }

        public override void Close()
        {
            GetComponentInUnit<ChestAnimation>().ChangeAnimationWithSpeed(closeClip);
            isOpened = false;
            Enable();
        }

        private void Enable()
        {
            if(isOpened) return;
            
            hotkeyUI.Show();
            isInteractable = true;
        }

        private void Disable()
        {
            isInteractable = false;
            hotkeyUI.Hide();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(Layers.PLAYER_LAYER, other.gameObject.layer))
                return;

            Enable();
        }

        private void OnTriggerExit(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(Layers.PLAYER_LAYER, other.gameObject.layer))
                return;

            Disable();
        }
    }
}
