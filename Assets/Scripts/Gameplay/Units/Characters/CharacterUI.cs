using System;
using Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterUI : UnitUI
    {
        [Inject] protected DiContainer diContainer;
        
        [SerializeField] protected CharacterMainController characterMainController;
        [SerializeField] protected HealthBarUI healthBarUI;
        [SerializeField] protected EnduranceBarUI enduranceBarUI;
        [SerializeField] protected ManaBarUI manaBarUI;
        

        public virtual void Initialize()
        {
            diContainer.Inject(healthBarUI);
            healthBarUI.Initialize();
            
            diContainer.Inject(enduranceBarUI);
            enduranceBarUI.Initialize();
            
            diContainer.Inject(manaBarUI);
            manaBarUI.Initialize();
        }

        public virtual void OnChangedHealth(float current, float max)
        {
            healthBarUI.UpdateHealthBar(current, max);
            healthBarUI.UpdateGradient();
        }
        
        public virtual void OnChangedEndurance(float current, float max)
        {
            enduranceBarUI.UpdateHealthBar(current, max);
            enduranceBarUI.UpdateGradient();
        }

        public virtual void OnChangedMana(float current, float max)
        {
            manaBarUI.UpdateHealthBar(current, max);
            manaBarUI.UpdateGradient();
        }
    }
}