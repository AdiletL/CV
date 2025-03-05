using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIItem : MonoBehaviour, IPointerClickHandler
    {
        public event Action<int?> OnClickedLeftMouse;
        public event Action<int?> OnClickedRightMouse;
        
        [SerializeField] private Image icon;
        [SerializeField] private Image cooldownBar;
        [SerializeField] private TextMeshProUGUI amountTxt;
        [SerializeField] private TextMeshProUGUI cooldownText;

        private float lastTime = -1f;
        private bool isInteractable;
        
        public int? SlotID { get; private set; }
        
        public void Initialize()
        {
        }
        public void SetSlotID(int? slotID) => SlotID = slotID;
        
        public void Clear()
        {
            SlotID = null;
            UpdateIcon(null);
            UpdateAmount(0);
            UpdateSelectable(false);
            UpdateCooldownBar(0, 0);
        }

        public void UpdateIcon(Sprite sprite)
        {
            icon.enabled = sprite != null;
            icon.sprite = sprite;
        }

        public void UpdateAmount(int amount)
        {
            if (amount <= 0) amountTxt.enabled = false;
            else amountTxt.enabled = true;
            
            amountTxt.text = amount.ToString();
        }
        public void UpdateSelectable(bool value) => isInteractable = value;

        public void UpdateCooldownBar(float current, float max)
        {
            if (max <= 0 || current <= 0)
            {
                if (cooldownText.enabled)
                {
                    cooldownBar.fillAmount = 0;
                    cooldownText.enabled = false;
                }
                return;
            }

            if (Mathf.Approximately(lastTime, current)) return;
            
            lastTime = current;
            cooldownBar.fillAmount = current/max;
            cooldownText.text = current.ToString("0");
            
            if(!cooldownText.enabled) cooldownText.enabled = true;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!isInteractable) return;
            
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnClickedRightMouse?.Invoke(SlotID);
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnClickedLeftMouse?.Invoke(SlotID);
            }
        }
    }
}