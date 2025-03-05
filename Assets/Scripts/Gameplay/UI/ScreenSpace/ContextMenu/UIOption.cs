using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.ScreenSpace.ContextMenu
{
    public class UIOption : MonoBehaviour
    {
        public event Action<int> OnSelected;
        
        [SerializeField] private TextMeshProUGUI headerTxt;
        [SerializeField] private Button button;

        public int SlotID { get; private set; }
        
        private void Start()
        {
            button.onClick.AddListener(Select);
        }
        
        private void Select() => OnSelected?.Invoke(SlotID);
        
        public void SetSlotID(int slotID) => SlotID = slotID;
        public void SetHeader(string header) => headerTxt.text = header;
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}