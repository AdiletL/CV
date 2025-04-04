using UnityEngine;

namespace Gameplay.UI
{
    public class EvasionPopUp : PopUpUI
    {
        private const string MISS_NAME = "MISS";
        
        public void Play()
        {
            valueText.text = MISS_NAME;
            elapsedTime = 0f;

            transform.localScale = startScale;
            textColor = valueText.color;
            textColor.a = 1f;
            valueText.color = textColor;

            float randomX = randomValuesX[Random.Range(0, randomValuesX.Length)];
            startPosition = transform.position + (Vector3.right * randomX);
            endPosition = startPosition - new Vector3(0, fallDistance, 0);
            transform.position = startPosition;

            isAnimating = true;
        }
    }
}