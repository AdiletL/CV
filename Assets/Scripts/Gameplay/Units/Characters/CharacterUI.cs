using System;
using ScriptableObjects.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Unit.Character
{
    public abstract class CharacterUI : UnitUI, ICharacter
    {
        [SerializeField] protected CharacterMainController characterMainController;
        [SerializeField] protected Image healthBar;
        [SerializeField] protected Image enduranceBar;
        
        protected SO_GameUIColor so_GameUIColor;
        
        protected Gradient healthBarGradient;
        protected Gradient enduranceBarGradient;
        protected float resultHealth;
        protected float resultEndurance;

        [Inject]
        public void Construct(SO_GameUIColor so_GameUIColor)
        {
            this.so_GameUIColor = so_GameUIColor;
        }
        
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
            healthBar.material.color = healthBarGradient.Evaluate(resultHealth);
        }

        public virtual void OnChangedEndurance(EnduranceInfo enduranceInfo)
        {
            UpdateEnduranceBar(enduranceInfo.CurrentEndurance, enduranceInfo.MaxEndurance);
        }
        protected virtual void UpdateEnduranceBar(float current, float max)
        {
            resultEndurance = current/max;
            enduranceBar.fillAmount = resultEndurance;
            enduranceBar.material.color = enduranceBarGradient.Evaluate(resultEndurance);
        }
    }
}