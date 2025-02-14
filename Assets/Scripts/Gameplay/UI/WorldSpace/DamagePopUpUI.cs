using System.Collections;
using UnityEngine;

namespace Gameplay.UI
{
    public class DamagePopUpUI : PopUpUI
    {
        [SerializeField] private AnimationCurve sizeCurve;
        [SerializeField] private AnimationCurve fallCurve;
        
        [SerializeField] private Vector3 startScale = new Vector3(1.5f, 1.5f, 1);
        [SerializeField] private Vector3 endScale = new Vector3(1f, 1f, 1);
        [SerializeField] private float fallDistance = 1f;

        private float elapsedTime;
        private Vector3 startPosition;
        private Vector3 endPosition;

        private static readonly float[] RandomValues = { -0.15f, -0.1f, -0.05f, 0.05f, 0.1f, 0.15f };

        public override void Play(int damageValue)
        {
            valueText.text = damageValue.ToString();
            elapsedTime = 0f;

            transform.localScale = startScale;
            textColor = valueText.color;
            textColor.a = 1f;
            valueText.color = textColor;

            float randomX = RandomValues[Random.Range(0, RandomValues.Length)];
            startPosition = transform.position + (Vector3.right * randomX);
            endPosition = startPosition - new Vector3(0, fallDistance, 0);

            isAnimating = true;
        }

        private void Update()
        {
            if (!isAnimating) return;

            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;

            textColor.a = alphaCurve.Evaluate(progress);
            valueText.color = textColor;

            transform.localScale = Vector3.Lerp(startScale, endScale, sizeCurve.Evaluate(progress));
            transform.position = Vector3.Lerp(startPosition, endPosition, fallCurve.Evaluate(progress));

            if (elapsedTime >= animationDuration)
            {
                isAnimating = false;
                textColor.a = 0f;
                valueText.color = textColor;
                transform.localScale = endScale;
                transform.position = endPosition;

                if (poolManager != null)
                    poolManager.ReturnToPool(gameObject);
                else
                    Destroy(gameObject);
            }
        }
    }
}
