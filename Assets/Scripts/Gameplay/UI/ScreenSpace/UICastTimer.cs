using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.ScreenSpace
{
    public class UICastTimer : MonoBehaviour
    {
        [SerializeField] private Transform parent;
        [SerializeField] private TextMeshProUGUI timerTxt;
        [SerializeField] private Image timerBar;

        private float lastTime = -1f;

        public void Initialize()
        {
            Hide();
        }

        public void UpdateTime(float currentTime, float maxTimer)
        {
            if (currentTime <= 0 || maxTimer <= 0)
            {
                if (parent.gameObject.activeSelf) // Проверяем, не скрыт ли уже
                    Hide();
                return;
            }

            if (Mathf.Approximately(lastTime, currentTime)) return; // Если время не изменилось, выходим

            lastTime = currentTime; // Запоминаем последнее обновленное значение
            timerTxt.text = currentTime.ToString("0.0");
            timerBar.fillAmount = currentTime / maxTimer;
    
            if (!parent.gameObject.activeSelf) // Проверяем, скрыт ли объект, перед тем как показать
                Show();
        }
        
        public void Show() => parent.gameObject.SetActive(true);
        public void Hide() => parent.gameObject.SetActive(false);
    }
}