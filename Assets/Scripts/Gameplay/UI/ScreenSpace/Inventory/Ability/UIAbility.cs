using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIAbility : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<int?> OnClickedLeftMouse;
        public event Action<int?> OnEnter;
        public event Action OnExit;
        
        [SerializeField] private Image icon;
        [SerializeField] private Image cooldownBar;
        [SerializeField] private TextMeshProUGUI cooldownText;
        [SerializeField] private Button button;

        private float lastTime = -1f;
        
        public int? SlotID { get; private set; }
        
        
        public void Initialize()
        {
            button.onClick.AddListener(Select);
        }

        public void Clear()
        {
            SetIcon(null);
            SetSlotID(null);
            SetCooldownBar(0, 0);
            SetSelectable(false);
        }

        public void SetSlotID(int? slotID) => SlotID = slotID;
        public void SetIcon(Sprite sprite)
        { 
            icon.enabled = sprite != null;
            icon.sprite = sprite;
        }

        public void SetCooldownBar(float current, float max)
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


        public void SetSelectable(bool isReady)
        {
            button.interactable = isReady;
        }

        private void Select()
        {
            OnClickedLeftMouse?.Invoke(SlotID);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnter?.Invoke(SlotID);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExit?.Invoke();
        }
    }
}