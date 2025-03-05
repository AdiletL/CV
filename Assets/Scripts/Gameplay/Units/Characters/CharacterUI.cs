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

        public virtual void OnChangedHealth(float current, float max)
        {
            UpdateHealthBar(current, max);
        }

        protected virtual void UpdateHealthBar(float current, float max)
        {
            resultHealth = current/max;
            healthBar.fillAmount = resultHealth;
            healthBar.color = healthBarGradient.Evaluate(resultHealth);
        }

        public virtual void OnChangedEndurance(float current, float max)
        {
            UpdateEnduranceBar(current, current);
        }
        protected virtual void UpdateEnduranceBar(float current, float max)
        {
            resultEndurance = current/max;
            enduranceBar.fillAmount = resultEndurance;
            enduranceBar.color = enduranceBarGradient.Evaluate(resultEndurance);
        }
    }
}