using System;
using UnityEngine;
using Zenject;

namespace Gameplay.Skill
{
    public class Dash : Skill
    {
        private float dashTimer;
        private bool isDashing;
        
        public CharacterController CharacterController { get; set; }
        public float Duration { get; set; }
        public float Speed { get; set; }

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
                CharacterController.Move(GameObject.transform.forward * Speed * Time.deltaTime);
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
        
        public DashBuilder SetCharacterController(CharacterController characterController)
        {
            if(skill is Dash dash)
                dash.CharacterController = characterController;
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