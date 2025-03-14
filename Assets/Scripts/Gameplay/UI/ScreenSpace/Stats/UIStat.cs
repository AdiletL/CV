using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.ScreenSpace.Stats
{
    public class UIStat : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI txt;
        [SerializeField] private Image bar;
        
        public void SetIcon(Sprite icon) => image.sprite = icon;
        public void SetText(string value) => txt.text = value;
        public void SetBar(float value) => bar.fillAmount = value;
        public void SetGradientBar(Color color) => bar.color = color;
    }
}