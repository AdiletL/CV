using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Unit.Cell;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Trap.Fall
{
    public class FallGravityController : TrapController, IFallGravity
    {
        [Inject] private SO_GameConfig gameConfig;
        
        private SO_FallGravity so_FallGravity;
        private Coroutine checkAndAddRigidBodyCoroutine;
        
        private float intervalFallObjects;
        private float radius;
        
        
        public float Mass { get; private set; }


        public override void Initialize()
        {
            base.Initialize();

            so_FallGravity = (SO_FallGravity)so_Trap;
            Mass = so_FallGravity.Mass;
            intervalFallObjects = so_FallGravity.IntervalFallObjets;
            if(TryGetComponent(out SphereCollider sphereCollider))
                radius = sphereCollider.radius;
        }
        
        public override void Appear()
        {
        }

        public override void Disappear()
        {
            
        }

        public override void StartAction()
        {
            if(isStarted) return;
            ExecuteFallGravity();
        }

        public void ExecuteFallGravity()
        {
            if(checkAndAddRigidBodyCoroutine != null) StopCoroutine(checkAndAddRigidBodyCoroutine);
            checkAndAddRigidBodyCoroutine = StartCoroutine(CheckInRadiusAndAddRigidbodyCoroutine());
        }
        
        private IEnumerator CheckInRadiusAndAddRigidbodyCoroutine()
        {
            var colliders = Physics.OverlapSphere(transform.position, radius);
            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                if (colliders[i].TryGetComponent(out IFallatable fallatable))
                {
                    fallatable.ActivateFall(Mass);
                    yield return new WaitForSeconds(intervalFallObjects);
                }
            }
        }
    }
}