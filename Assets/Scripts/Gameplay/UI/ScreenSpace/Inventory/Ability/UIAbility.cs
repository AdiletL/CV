using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIAbility : MonoBehaviour
    {
        public static Action<int?> OnSlotSelected;
        
        [SerializeField] private Image icon;
        [SerializeField] private Image cooldownBar;
        [SerializeField] private Button button;

        private float resultCooldown;
        
        public int? SlotID { get; private set; }
        
        
        public void Initialize()
        {
            button.onClick.AddListener(Select);
        }

        public void Clear()
        {
            SetIcon(null);
            SetSlotID(null);
            UpdateCooldownBar(0, 1);
            UpdateSelectable(false);
        }

        public void SetSlotID(int? slotID) => SlotID = slotID;
        public void SetIcon(Sprite sprite)
        { 
            icon.enabled = sprite != null;
            icon.sprite = sprite;
        }

        public void UpdateCooldownBar(float current, float max)
        {
            if (max <= 0)
            {
                cooldownBar.fillAmount = 0;
                return;
            }
            resultCooldown = current/max;
            cooldownBar.fillAmount = resultCooldown;
        }


        public void UpdateSelectable(bool isReady)
        {
            button.interactable = isReady;
        }

        private void Select()
        {
            OnSlotSelected?.Invoke(SlotID);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}