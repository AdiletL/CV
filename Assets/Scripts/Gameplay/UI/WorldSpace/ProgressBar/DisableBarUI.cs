using TMPro;
using UnityEngine;

namespace Gameplay.UI
{
    public class DisableBarUI : ProgressBarUI
    {
        [SerializeField] private TextMeshProUGUI headerTxt;
        public override void Initialize()
        {
            gradient = so_GameUIColor.DisableBarGradient;
        }
        
        public void SetText(string text) => headerTxt.text = text;
    }
}