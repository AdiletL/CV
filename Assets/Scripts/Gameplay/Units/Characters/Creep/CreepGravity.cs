using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Unit.Character.Creep
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class CreepGravity : Gravity
    {
        [SerializeField] private Transform startRayPointsParent;
        
        private NavMeshAgent navMeshAgent;
        private Rigidbody rigidBody;
        private RaycastHit hit;
        
        private List<Transform> startRayPoints = new();
        private const float rayDistance = 1;
        
        private bool IsGrounded()
        {
            foreach (var VARIABLE in startRayPoints)
            {
                if (Physics.Raycast(VARIABLE.position, Vector3.down, out hit, rayDistance))
                    return true;
            }
            return false;
        }
        
        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            rigidBody = GetComponent<Rigidbody>();
            CurrentGravity = Physics.gravity.y;
            
            for (int i = 0; i < startRayPointsParent.childCount; i++)
                startRayPoints.Add(startRayPointsParent.GetChild(i));
        }

        private void LateUpdate()
        {
            UseGravity();
        }

        protected override void UseGravity()
        {
            if (IsGrounded())
            {
                rigidBody.useGravity = false;
                if(navMeshAgent.isOnNavMesh)
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