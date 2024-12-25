using System;
using System.Collections.Generic;
using Unit.Character.Player;
using UnityEngine;

namespace Unit.Trap.Activator
{
    [RequireComponent(typeof(SphereCollider))]
    public class ButtonController : ActivatorController
    {
        private Animator animator;
        private SphereCollider sphereCollider;

        private bool isReady = true;
        
        private const string ACTIVATE_NAME = "Activate";
        private const string DEACTIVATE_NAME = "Deactivate";

        public override void Initialize()
        {
            base.Initialize();
            
            animator = GetComponent<Animator>();
        }

        public override void Activate()
        {
            base.Activate();
            isReady = false;
            animator.SetTrigger(ACTIVATE_NAME);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            isReady = true;
            animator.SetTrigger(DEACTIVATE_NAME);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!isReady) return;
            
            if (other.TryGetComponent(out PlayerController playerController))
            {
                Activate();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(isReady) return;
            
            if (other.TryGetComponent(out PlayerController playerController))
            {
                Deactivate();
            }
        }
    }
}