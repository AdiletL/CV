using System;
using System.Collections;
using Photon.Pun;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public abstract class TrapController : UnitController, ITrap
    {
        [SerializeField] protected SO_Trap so_Trap;

        protected PhotonView photonView;
        protected TrapAnimation trapAnimation;

        protected Coroutine activationDelayCoroutine;
        protected Coroutine cooldownCoroutine;
        
        protected AnimationClip playClip;
        protected AnimationClip resetClip;
        
        protected float activationDelay;
        protected float cooldown;
        protected bool isStarted;
        protected bool isReusable;

        public LayerMask EnemyLayer { get; protected set; }
        
        public override void Initialize()
        {
            base.Initialize();
            playClip = so_Trap.PlayClip;
            resetClip = so_Trap.ResetClip;
            EnemyLayer = so_Trap.EnemyLayer;
            activationDelay = so_Trap.ActivationDelay;
            cooldown = so_Trap.Cooldown;
            isReusable = so_Trap.IsReusable;

            trapAnimation = GetComponentInUnit<TrapAnimation>();
            if(trapAnimation) trapAnimation.Initialize();
            if(playClip) trapAnimation.AddClip(playClip);
            if(resetClip) trapAnimation.AddClip(resetClip);
        }

        public virtual void StartAction()
        {
            if(isStarted) return;
            if(activationDelayCoroutine != null) StopCoroutine(activationDelayCoroutine);
            activationDelayCoroutine = StartCoroutine(ActivationDelayCoroutine());
        }

        private IEnumerator ActivationDelayCoroutine()
        {
            isStarted = true;
            yield return new WaitForSeconds(activationDelay);
            trapAnimation.ChangeAnimationWithDuration(playClip);
        }
        
        public virtual void ResetAction()
        {
            trapAnimation.ChangeAnimationWithDuration(resetClip);
        }

        public void StartCooldown()
        {
            if(!isStarted) return;
            if(cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = StartCoroutine(CooldownCoroutine());
        }
        private IEnumerator CooldownCoroutine()
        {
            yield return new WaitForSeconds(cooldown);
            if(isReusable) isStarted = false;
        }
    }
}