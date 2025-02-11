using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIItem : MonoBehaviour
    {
        public static Action<string> OnItemSelected;
        
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Button button;
        
        [HideInInspector] public string Name;
        
        public void Initialize()
        {
            button.onClick.AddListener(Select);
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
                Clear();
            }
            else
            {
                text.enabled = true;
                text.text = value.ToString();
            }
        }

        public void SetCanSelect(bool value)
        {
            button.interactable = value;
        }

        public void Clear()
        {
            Name = string.Empty;
            SetIcon(null);
            text.enabled = false;
            button.interactable = false;
        }

        public void Select()
        {
            OnItemSelected?.Invoke(Name);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}