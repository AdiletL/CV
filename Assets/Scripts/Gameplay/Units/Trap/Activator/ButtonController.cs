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

        private Coroutine checkTargetCoroutine, startTimerCoroutine;

        private float cooldownCheck = 1f, countCooldownCheck;

        private bool isReady = true;
        
        public override void Initialize()
        {
            base.Initialize();

            buttonAnimation = components.GetComponentFromArray<ButtonAnimation>();
        }

        public override void Appear()
        {
            
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

        public override void Deactivate()
        {
            base.Deactivate();
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            if(checkTargetCoroutine != null)
                StopCoroutine(checkTargetCoroutine);
            isReady = true;
            buttonAnimation.ChangeAnimationWithDuration(deactivateClip);
        }

        private IEnumerator StartTimerCoroutine(float waitTime, Action callback)
        {
            yield return new WaitForSeconds(waitTime);
            callback?.Invoke();
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

        private void OnTriggerEnter(Collider other)
        {
            if(!isReady 
               || !Calculate.GameLayer.IsTarget(EnemyLayers, other.gameObject.layer)
               || CurrentTarget) return;
            
            CurrentTarget = other.gameObject;
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(0.3f, Activate));
        }

        private void OnTriggerExit(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(EnemyLayers, other.gameObject.layer)
               || !CurrentTarget) return;
            
            Deactivate();
            CurrentTarget = null;
        }
    }
}