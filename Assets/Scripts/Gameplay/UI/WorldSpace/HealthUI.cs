using ScriptableObjects.Gameplay;
using Unit;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.UI
{
    public class HealthUI : MonoBehaviour
    {
        [Inject] private SO_GameUIColor so_GameUIColor;
        
        [SerializeField] protected Image healthBar;
        
        private Gradient healthBarGradient;
        private float resultHealth;

        public void Initialize()
        {
            healthBarGradient = so_GameUIColor.HealthBarGradient;

        }
        
        public virtual void OnChangedHealth(HealthInfo healthInfo)
        {
            UpdateHealthBar(healthInfo.CurrentHealth, healthInfo.MaxHealth);
        }

        protected virtual void UpdateHealthBar(int current, int max)
        {
            resultHealth = (float)current/max;
            healthBar.fillAmount = resultHealth;
            healthBar.color = healthBarGradient.Evaluate(resultHealth);
        }
    }
}