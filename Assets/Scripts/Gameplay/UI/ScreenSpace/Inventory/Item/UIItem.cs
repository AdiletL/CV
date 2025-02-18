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
        
        public void Clear()
        {
            SlotID = null;
            SetIcon(null);
            text.enabled = false;
            ChangeReadiness(false);
            UpdateCooldownBar(0, 1);
        }

        public void SetIcon(Sprite sprite)
        {
            icon.enabled = sprite != null;
            icon.sprite = sprite;
        }

        public void SetValue(int value)
        {
            if (value == 0)
            {
                text.text = value.ToString();
            }
            else
            {
                text.enabled = true;
                text.text = value.ToString();
            }
        }

        public void ChangeReadiness(bool value) => button.interactable = value;
        public void SetSlotID(int? slotID) => SlotID = slotID;

        public void UpdateCooldownBar(float current, float max)
        {
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