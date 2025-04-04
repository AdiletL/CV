using Gameplay.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay.UI
{
    public abstract class PopUpUI : MonoBehaviour
    {
        [Inject] protected PoolManager poolManager;
        
        [SerializeField] protected TextMeshProUGUI valueText;
        [SerializeField] protected AnimationCurve alphaCurve;
        [SerializeField] protected float animationDuration = 1.0f;
        [SerializeField] protected AnimationCurve sizeCurve;
        [SerializeField] protected AnimationCurve fallCurve;
        
        [SerializeField] protected Vector3 startScale = new Vector3(1.5f, 1.5f, 1);
        [SerializeField] protected Vector3 endScale = new Vector3(1f, 1f, 1);
        [SerializeField] protected float fallDistance = 1f;

        protected float elapsedTime;
        protected Vector3 startPosition;
        protected Vector3 endPosition;
        
        protected Color textColor;
        protected bool isAnimating;

        protected float[] randomValuesX;
        protected float[] randomValuesY;
        protected float[] randomValuesZ;

        protected virtual float[] CreateRandomValuesForPositionX()
        {
            return new float[] { -0.2f, -0.15f, -0.1f, 0.1f, 0.15f, 0.2f };
        }
        protected virtual float[] CreateRandomValuesForPositionY()
        {
            return new float[] { 0 };
        }
        protected virtual float[] CreateRandomValuesForPositionZ()
        {
            return new float[] { 0 };
        }

        private void Awake()
        {
            randomValuesX = CreateRandomValuesForPositionX();
            randomValuesY = CreateRandomValuesForPositionY();
            randomValuesZ = CreateRandomValuesForPositionZ();
        }

        public virtual void Play(float damageValue)
        {
            valueText.text = damageValue.ToString("0");
            elapsedTime = 0f;

            transform.localScale = startScale;
            textColor = valueText.color;
            textColor.a = 1f;
            valueText.color = textColor;

            float randomX = randomValuesX[Random.Range(0, randomValuesX.Length)];
            float randomY = randomValuesY[Random.Range(0, randomValuesY.Length)];
            float randomZ = randomValuesZ[Random.Range(0, randomValuesZ.Length)];

            startPosition = transform.position + new Vector3(randomX, randomY, randomZ);
            endPosition = startPosition - new Vector3(0, fallDistance, 0);
            transform.position = startPosition;

            isAnimating = true;
        }

        protected virtual void Update()
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

                if(poolManager)
                    poolManager.ReturnToPool(gameObject);
                else
                    Destroy(gameObject);
            }
        }
    }
}