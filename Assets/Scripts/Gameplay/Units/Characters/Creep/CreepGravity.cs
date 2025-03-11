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
        [SerializeField] private CheckGrounded[] checksGrounded;
        
        private NavMeshAgent navMeshAgent;
        private Rigidbody rigidBody;
        
        public override bool IsGrounded
        {
            get
            {
                for (int i = 0; i < checksGrounded.Length; i++)
                {
                    if (checksGrounded[i].IsGrounded())
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
        }

        private void FixedUpdate()
        {
            UseGravity();
        }

        protected override void UseGravity()
        {
            if (IsGrounded)
            {
                rigidBody.useGravity = false;
                rigidBody.isKinematic = true;
                navMeshAgent.enabled = true;
            }
            else
            {
                if(!isGravity) return;
                navMeshAgent.enabled = false;
                rigidBody.isKinematic = false;
                rigidBody.useGravity = true;
            }
        }
    }
}