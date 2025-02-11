using System.Collections;
using Gameplay.Manager;
using TMPro;
using UnityEngine;
using Zenject;

namespace Gameplay.UI
{
    public class DamageUI : MonoBehaviour
    {
        [Inject] private PoolManager poolManager;
        
        [SerializeField] private TextMeshProUGUI damageText;

        [SerializeField] private AnimationCurve alphaCurve;
        [SerializeField] private AnimationCurve sizeCurve;
        [SerializeField] private AnimationCurve fallCurve;
        
        [SerializeField] private Vector3 startScale = new Vector3(1.5f, 1.5f, 1);
        [SerializeField] private Vector3 endScale = new Vector3(1f, 1f, 1);
        [SerializeField] private float animationDuration = 1.0f;
        [SerializeField] private float fallDistance = 1f;

        private float elapsedTime;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private Color textColor;
        private bool isAnimating;

        private static readonly float[] RandomValues = { -0.15f, -0.1f, -0.05f, 0.05f, 0.1f, 0.15f };

        public void Play(int damageValue)
        {
            damageText.text = damageValue.ToString();
            elapsedTime = 0f;

            transform.localScale = startScale;
            textColor = damageText.color;
            textColor.a = 1f;
            damageText.color = textColor;

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
            damageText.color = textColor;

            transform.localScale = Vector3.Lerp(startScale, endScale, sizeCurve.Evaluate(progress));
            transform.position = Vector3.Lerp(startPosition, endPosition, fallCurve.Evaluate(progress));

            if (elapsedTime >= animationDuration)
            {
                isAnimating = false;
                textColor.a = 0f;
                damageText.color = textColor;
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
