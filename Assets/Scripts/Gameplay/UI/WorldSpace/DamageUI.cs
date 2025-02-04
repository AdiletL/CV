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

        private Coroutine startAnimateCoroutine;
        private float elapsedTime;
        
        private float[] randomValues = new []
        {
            -.15f, -.1f, -.05f, .05f, .1f, .15f
        };
        
        public void Play(int damageValue)
        {
            damageText.text = damageValue.ToString();
            elapsedTime = 0f;

            transform.localScale = startScale;
            if(startAnimateCoroutine != null) StopCoroutine(startAnimateCoroutine);
            startAnimateCoroutine = StartCoroutine(StartAnimateCoroutine());
        }

        private IEnumerator StartAnimateCoroutine()
        {
            var randomX = randomValues[Random.Range(0, randomValues.Length)];
            Vector3 startPosition = transform.position + (Vector3.right * randomX);
            Vector3 endPosition = startPosition - new Vector3(0, fallDistance, 0);

            Color textColor = damageText.color;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / animationDuration;

                textColor.a = alphaCurve.Evaluate(progress);
                damageText.color = textColor;

                transform.localScale = Vector3.Lerp(startScale, endScale, sizeCurve.Evaluate(progress));

                transform.position = Vector3.Lerp(startPosition, endPosition, fallCurve.Evaluate(progress));

                yield return null;
            }

            // Ensure final state
            textColor.a = 0f;
            damageText.color = textColor;
            transform.localScale = endScale;
            transform.position = endPosition;

            if(poolManager != null)
                poolManager.ReturnToPool(gameObject); // Or return to pool if pooling is used
            else
                Destroy(gameObject);
        }
    }
}