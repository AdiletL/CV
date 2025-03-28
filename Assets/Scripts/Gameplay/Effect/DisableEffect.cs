using System;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Gameplay.Effect
{
    public class DisableEffect : Effect, IDisable
    {
        public event Action<float, float> OnCountTimer;
        public event Action<IDisable> OnDestroyDisable;

        public override EffectType EffectTypeID { get; } = EffectType.Disable;
        
        public DisableType DisableTypeID { get; }
        public float Timer { get; }
        public float CountTimer { get; protected set; }

        private bool isActive;
        
        
        public DisableEffect(DisableConfig disableConfig) : base(disableConfig)
        {
            DisableTypeID = disableConfig.DisableTypeID;
            Timer = disableConfig.Timer;
        }

        public override void ClearValues()
        {
            CountTimer = Timer;
        }

        public override void Update()
        {
            if(!isActive) return;
           
            CountTimer -= Time.deltaTime;
            if (CountTimer <= 0)
            {
                OnDestroyDisable?.Invoke(this);
                isActive = false;
            }
            OnCountTimer?.Invoke(CountTimer, Timer);
        }

        public override void FixedUpdate()
        {
            
        }

        public override void UpdateEffect()
        {
            CountTimer = Timer;
        }

        public override void ApplyEffect()
        {
            ClearValues();
            isActive = true;
        }

        public override void DestroyEffect()
        {
            base.DestroyEffect();
            isActive = false;
        }
    }

    [System.Serializable]
    public class DisableConfig : EffectConfig
    {
        public DisableType DisableTypeID;
        public float Timer;
    }
}