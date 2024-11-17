using System;
using ScriptableObjects.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Unit.Character
{
    public abstract class CharacterUI : MonoBehaviour, ICharacter
    {
        [SerializeField] protected CharacterMainController characterMainController;
        [SerializeField] protected Image healthBar;
        
        protected SO_GameUIColor so_GameUIColor;

        [Inject]
        public void Construct(SO_GameUIColor so_GameUIColor)
        {
            this.so_GameUIColor = so_GameUIColor;
        }
        
        public virtual void Initialize()
        {
            characterMainController.components.GetComponentFromArray<CharacterHealth>().OnChangedHealth += OnChangeHealth;
        }

        protected virtual void OnChangeHealth(IHealthInfo healthInfo)
        {
            UpdateHealthBar(healthInfo.CurrentHealth, healthInfo.MaxHealth);
        }

        protected virtual void UpdateHealthBar(int current, int max)
        {
            var result = (float)current/max;
            healthBar.fillAmount = result;
            healthBar.color = so_GameUIColor.healthBarGradient.Evaluate(result);
        }

        private void OnDestroy()
        {
            characterMainController.components.GetComponentFromArray<CharacterHealth>().OnChangedHealth -= OnChangeHealth;
        }
    }
}