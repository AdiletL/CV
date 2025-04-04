using System;
using UnityEngine;

namespace Gameplay.Ability
{
    public class DashAbility : Ability
    {
        public override AbilityType AbilityTypeID { get; protected set; } = AbilityType.Dash;
        
        private DashConfig dashConfig;
        private IMoveControl moveControl;
        private float duration;
        private float speed;
        private float dashTimer;
        private bool isStartEffect;

        public DashAbility(DashConfig dashConfig) : base(dashConfig)
        {
            this.dashConfig = dashConfig;
        }

        public void SetMoveControl(IMoveControl moveControl) => this.moveControl = moveControl;

        public override void Initialize()
        {
            base.Initialize();
            duration = dashConfig.Duration;
            speed = dashConfig.Speed;
        }

        public override void StartEffect()
        {
            base.StartEffect();
            if (isActivated)
            {
                dashTimer = duration;
                isStartEffect = true;
            }
        }

        public override void Update()
        {
            base.Update();
            if(isStartEffect)
                DashUpdate();
        }

        private void DashUpdate()
        {
            if (dashTimer > 0f)
            {
                dashTimer -= Time.deltaTime;
                moveControl.AddVelocity(GameObject.transform.forward * speed);
            }
            else
            {
                moveControl.ClearVelocity();
                isStartEffect = false;
                Exit();
            }
        }
    }
    
    [System.Serializable]
    public class DashConfig : AbilityConfig
    {
        public override AbilityType AbilityTypeID { get; } = AbilityType.Dash;
        public float Speed;
        public float Duration;
    }
}