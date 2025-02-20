using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIItem : MonoBehaviour
    {
        public static Action<int?> OnSlotSelected;
        
        [SerializeField] private Image icon;
        [SerializeField] private Image cooldownBar;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Button button;

        private float resultCooldown;
        
        public int? SlotID { get; private set; }
        
        public void Initialize()
        {
            button.onClick.AddListener(Select);
        }
        public void SetSlotID(int? slotID) => SlotID = slotID;
        
        public void Clear()
        {
            SlotID = null;
            UpdateIcon(null);
            UpdateAmount(0);
            UpdateSelectable(false);
            UpdateCooldownBar(0, 1);
        }

        public void UpdateIcon(Sprite sprite)
        {
            icon.enabled = sprite != null;
            icon.sprite = sprite;
        }

        public void UpdateAmount(int amount)
        {
            if (amount <= 0) text.enabled = false;
            else text.enabled = true;
            
            text.text = amount.ToString();
        }
        public void UpdateSelectable(bool value) => button.interactable = value;

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