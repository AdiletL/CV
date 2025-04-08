using System;
using ScriptableObjects.Ability;
using UnityEngine;

namespace Gameplay.Ability
{
    public class DashAbility : Ability
    {
        public override AbilityType AbilityTypeID { get; protected set; } = AbilityType.Dash;
        
        private SO_DashAbility so_DashAbility;
        private IMoveControl moveControl;
        private float duration;
        private float speed;
        private float dashTimer;
        private bool isStartEffect;

        public DashAbility(SO_DashAbility so_DashAbility) : base(so_DashAbility)
        {
            this.so_DashAbility = so_DashAbility;
        }

        public void SetMoveControl(IMoveControl moveControl) => this.moveControl = moveControl;

        public override void Initialize()
        {
            base.Initialize();
            duration = so_DashAbility.Duration;
            speed = so_DashAbility.Speed;
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
}