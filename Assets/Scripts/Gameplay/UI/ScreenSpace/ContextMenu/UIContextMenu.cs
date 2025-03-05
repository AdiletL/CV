using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

namespace Gameplay.UI.ScreenSpace.ContextMenu
{
    public class UIContextMenu : MonoBehaviour
    {
        public event Action<int> OnOptionSelected;
        
        [SerializeField] private AssetReferenceGameObject optionPrefab;
        [SerializeField] private Transform container;
        
        private List<UIOption> options = new();

        private UIOption CreateOption()
        {
            var newGameObject = Addressables.InstantiateAsync(optionPrefab, container).WaitForCompletion();
            return newGameObject.GetComponent<UIOption>();
        }
        
        // Оптимизированный метод для проверки, был ли клик по UI
        private bool IsPointerOverUIOption()
        {
            // Используем заранее созданные объекты
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            // Список результатов Raycast
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            // Получаем все результаты Raycast
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            foreach (RaycastResult raycastResult in raycastResults)
            {
                if (raycastResult.gameObject.TryGetComponent(out UIOption option))
                    return true;
            }
            // Проверяем, есть ли хотя бы один объект в UI
            return false;
        }
        
        public void AddOptions(string[] header)
        {
            for (int i = 0; i < header.Length; i++)
            {
                if (options.Count <= i)
                {
                    options.Add(CreateOption());
                    options[i].SetSlotID(i);
                    options[i].OnSelected += OnSelect;
                }
                options[i].SetHeader(header[i]);
                options[i].Show();
            }

            Show();
        }

        public void HideOptions()
        {
            foreach (var VARIABLE in options)
                VARIABLE.Hide();
            Hide();
        }
        
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        private void Update()
        {
            if (Input.anyKeyDown && gameObject.activeSelf)
            {
                if(!IsPointerOverUIOption())
                    Hide();
            }
        }
        
        private void OnSelect(int slotID) => OnOptionSelected?.Invoke(slotID);
    }
}