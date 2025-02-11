using System;
using ScriptableObjects.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Unit.Character
{
    public abstract class CharacterUI : UnitUI
    {
        [Inject] protected SO_GameUIColor so_GameUIColor;
        
        [SerializeField] protected CharacterMainController characterMainController;
        [SerializeField] protected Image healthBar;
        [SerializeField] protected Image enduranceBar;
        
        protected Gradient healthBarGradient;
        protected Gradient enduranceBarGradient;
        protected float resultHealth;
        protected float resultEndurance;
        
        
        public virtual void Initialize()
        {
            healthBarGradient = so_GameUIColor.HealthBarGradient;
            enduranceBarGradient = so_GameUIColor.EnduranceBarGradient;
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

        public virtual void OnChangedEndurance(EnduranceInfo enduranceInfo)
        {
            UpdateEnduranceBar(enduranceInfo.CurrentEndurance, enduranceInfo.MaxEndurance);
        }
        protected virtual void UpdateEnduranceBar(float current, float max)
        {
            resultEndurance = current/max;
            enduranceBar.fillAmount = resultEndurance;
            enduranceBar.color = enduranceBarGradient.Evaluate(resultEndurance);
        }
    }
}