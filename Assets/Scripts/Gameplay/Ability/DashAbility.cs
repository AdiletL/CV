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

        public override void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            base.Enter(finishedCallBack, target, point);
            if(!isActivated) return;
            dashTimer = duration;
            StartEffect();
        }
        
        public override void LateUpdate()
        {
            if (isActivated)
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
                Exit();
            }
        }
    }
    
    [System.Serializable]
    public class DashConfig : AbilityConfig
    {
        public float Speed;
        public float Duration;
    }
}