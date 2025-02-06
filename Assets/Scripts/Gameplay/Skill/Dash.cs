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
        
        private PlayerKinematicControl playerKinematicControl;
        public float Duration { get; set; }
        public float Speed { get; set; }
        
        public void SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl) => this.playerKinematicControl = playerKinematicControl;

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
        
        private void DashUpdate()
        {
            if (dashTimer > 0f)
            {
                dashTimer -= Time.deltaTime;
                playerKinematicControl.SetVelocity(GameObject.transform.forward * Speed);
            }
            else
            {
                isDashing = false;
                Exit();
            }
        }
    }

    public class DashBuilder : SkillBuilder<Dash>
    {
        public DashBuilder() : base(new Dash())
        {
        }
        
        public DashBuilder SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl)
        {
            if(skill is Dash dash)
                dash.SetPlayerKinematicControl(playerKinematicControl);
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