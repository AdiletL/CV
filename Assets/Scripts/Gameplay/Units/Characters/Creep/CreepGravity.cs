using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Unit.Character.Creep
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class CreepGravity : Gravity
    {
        [SerializeField] private Transform startRayPointsParent;
        
        private NavMeshAgent navMeshAgent;
        private Rigidbody rigidBody;
        
        private List<Transform> startRayPoints = new();
        private const float rayDistance = 1;
        private int baseLayer;

        public override bool IsGrounded
        {
            get
            {
                foreach (var VARIABLE in startRayPoints)
                {
                    //Debug.DrawRay(VARIABLE.position, Vector3.down * rayDistance, Color.green);
                    if (Physics.Raycast(VARIABLE.position, Vector3.down, rayDistance, ~baseLayer))
                        return true;
                }

                return false;
            }
        }
        
        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            rigidBody = GetComponent<Rigidbody>();
            CurrentGravity = Physics.gravity.y;
            
            for (int i = 0; i < startRayPointsParent.childCount; i++)
                startRayPoints.Add(startRayPointsParent.GetChild(i));
            
            baseLayer = gameObject.layer;
        }

        private void LateUpdate()
        {
            UseGravity();
        }

        protected override void UseGravity()
        {
            if (IsGrounded)
            {
                rigidBody.useGravity = false;
                navMeshAgent.enabled = true;
            }
            else
            {
                if(!isGravity) return;
                navMeshAgent.enabled = false;
                rigidBody.useGravity = true;
            }
        }
    }
}