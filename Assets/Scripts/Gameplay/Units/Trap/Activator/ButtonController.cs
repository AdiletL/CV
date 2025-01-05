using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unit.Trap.Activator
{
    [RequireComponent(typeof(SphereCollider))]
    public class ButtonController : ActivatorController
    {
        private ButtonAnimation buttonAnimation;
        private SphereCollider sphereCollider;

        private Coroutine checkTargetCoroutine;

        private float cooldownCheck = 1f, countCooldownCheck;

        private bool isReady = true;
        
        public override void Initialize()
        {
            base.Initialize();

            buttonAnimation = components.GetComponentFromArray<ButtonAnimation>();
        }

        public override void Activate()
        {
            base.Activate();
            isReady = false;
            buttonAnimation.ChangeAnimationWithDuration(activateClip);
            
            if(checkTargetCoroutine != null)
                StopCoroutine(checkTargetCoroutine);
            
            checkTargetCoroutine = StartCoroutine(CheckTargetCoroutine());
        }

        private IEnumerator CheckTargetCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(cooldownCheck);
                if (isReady && Physics.OverlapSphere(transform.position, .3f, Layers.PLAYER_LAYER).Length > 0)
                {
                    Activate();
                }
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
            isReady = true;
            buttonAnimation.ChangeAnimationWithDuration(deactivateClip);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!isReady 
               || !Calculate.GameLayer.IsTarget(EnemyLayers, other.gameObject.layer)
               || CurrentTarget) return;
            
            CurrentTarget = other.gameObject;
            Activate();
        }

        private void OnTriggerExit(Collider other)
        {
            if(isReady 
               || !Calculate.GameLayer.IsTarget(EnemyLayers, other.gameObject.layer)
               || !CurrentTarget) return;
            
            Deactivate();
            CurrentTarget = null;
        }
    }
}