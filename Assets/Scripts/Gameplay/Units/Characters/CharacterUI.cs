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
        [SerializeField] protected DisableBarUI disableBarUI;

        public virtual void Initialize()
        {
            diContainer?.Inject(healthBarUI);
            healthBarUI?.Initialize();
            
            diContainer?.Inject(enduranceBarUI);
            enduranceBarUI?.Initialize();
            
            diContainer?.Inject(manaBarUI);
            manaBarUI?.Initialize();
            
            diContainer?.Inject(disableBarUI);
            disableBarUI?.Initialize();
            disableBarUI?.Hide();
        }

        public virtual void OnChangedHealth(float current, float max)
        {
            healthBarUI.UpdateBar(current, max);
            healthBarUI.UpdateGradient();
        }
        
        public virtual void OnChangedEndurance(float current, float max)
        {
            enduranceBarUI.UpdateBar(current, max);
            enduranceBarUI.UpdateGradient();
        }

        public virtual void OnChangedMana(float current, float max)
        {
            manaBarUI.UpdateBar(current, max);
            manaBarUI.UpdateGradient();
        }

        public virtual void OnTimerDisable(float current, float max)
        {
            disableBarUI.UpdateBar(current, max);
            disableBarUI.UpdateGradient();
        }
        
        public void ShowDisableBar() => disableBarUI.Show();
        public void HideDisableBar() => disableBarUI.Hide();
        public void SetTextDisableBar(string text) => disableBarUI.SetText(text);
    }
}