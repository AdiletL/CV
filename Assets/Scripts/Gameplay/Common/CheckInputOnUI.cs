using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay
{
    public static class CheckInputOnUI
    {
        // Оптимизированный метод для проверки, был ли клик по UI
        public static bool IsPointerOverUIObject()
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

            // Проверяем, есть ли хотя бы один объект в UI
            return raycastResults.Count > 0;
        }
    }
}