using System;
using UnityEngine;

namespace Gameplay.Ability
{
    public class DashAbility : Ability
    {
        private IMoveControl moveControl;
        private float duration;
        private float speed;
        private float dashTimer;
        
        public override AbilityType AbilityTypeID { get; protected set; } = AbilityType.Dash;
        
        public void SetMoveControl(IMoveControl moveControl) => this.moveControl = moveControl;
        public void SetDuration(float duration) => this.duration = duration;
        public void SetSpeed(float speed) => this.speed = speed;
        

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

    public class DashAbilityBuilder : AbilityBuilder
    {
        public DashAbilityBuilder() : base(new DashAbility())
        {
        }
        
        public DashAbilityBuilder SetDuration(float duration)
        {
            if(ability is DashAbility dash)
                dash.SetDuration(duration);
            return this;
        }
        public DashAbilityBuilder SetSpeed(float speed)
        {
            if(ability is DashAbility dash)
                dash.SetSpeed(speed);
            return this;
        }
    }
}