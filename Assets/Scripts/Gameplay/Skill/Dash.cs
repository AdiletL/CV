using System;
using Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Skill
{
    public class Dash : Skill
    {
        private float dashTimer;
        private bool isDashing;
        
        private IMoveControl moveControl;
        public override SkillType SkillType { get; protected set; } = SkillType.dash;
        
        public float Duration { get; set; }
        public float Speed { get; set; }
        
        public void SetMoveControl(IMoveControl moveControl) => this.moveControl = moveControl;

        public override void Execute(Action exitCallback = null)
        {
            base.Execute(exitCallback);
            isDashing = true;
            dashTimer = Duration;
        }

        public override void Update()
        {
            
        }

        public override void LateUpdate()
        {
            if (isDashing)
            {
                DashUpdate();
            }
        }

        public override void CheckTarget()
        {
            
        }

        private void DashUpdate()
        {
            if (dashTimer > 0f)
            {
                dashTimer -= Time.deltaTime;
                moveControl.AddVelocity(GameObject.transform.forward * Speed);
            }
            else
            {
                moveControl.ClearVelocity();
                isDashing = false;
                Exit();
            }
        }
    }
    
    [System.Serializable]
    public class DashConfig : SkillConfig
    {
        public float Speed;
        public float Duration;
    }

    public class DashBuilder : SkillBuilder<Dash>
    {
        public DashBuilder() : base(new Dash())
        {
        }
        
        public DashBuilder SetMoveControl(IMoveControl iMoveControl)
        {
            if(skill is Dash dash)
                dash.SetMoveControl(iMoveControl);
            return this;
        }

        public DashBuilder SetDuration(float duration)
        {
            if(skill is Dash dash)
                dash.Duration = duration;
            return this;
        }
        public DashBuilder SetSpeed(float speed)
        {
            if(skill is Dash dash)
                dash.Speed = speed;
            return this;
        }
    }
}