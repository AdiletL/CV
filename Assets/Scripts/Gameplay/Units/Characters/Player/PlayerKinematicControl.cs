using System;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unit.Character.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(KinematicCharacterMotor))]
    public class PlayerKinematicControl : MonoBehaviour, ICharacterController
    {
        [SerializeField] private KinematicCharacterMotor  motor;

        private Vector3 gravity;
        
        public Vector3 Velocity { get; private set; }
        public Vector3 DirectionRotate { get; private set; }
        public float RotationSpeed { get; private set; }
        
        public bool IsGrounded => motor.GroundingStatus.IsStableOnGround;

        private void Awake()
        {
            motor.CharacterController = this;
            gravity = new Vector3(0, Physics.gravity.y, 0);
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if(DirectionRotate.magnitude == 0) return;
            Quaternion toRotation = Quaternion.LookRotation(DirectionRotate, Vector3.up);
            currentRotation = Quaternion.RotateTowards(currentRotation, toRotation, RotationSpeed * deltaTime);
        }

        public void SetDirectionRotate(Vector3 direction)
        {
            DirectionRotate = direction;
        }

        public void SetRotationSpeed(float speed)
        {
            RotationSpeed = speed;
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (!IsGrounded)
                Velocity +=  gravity * deltaTime;
            else
                Velocity = new Vector3(Velocity.x, 0, Velocity.z);
            
            currentVelocity = Velocity;
        }

        public void SetVelocity(Vector3 newVelocity)
        {
            Velocity = new Vector3(newVelocity.x, Velocity.y, newVelocity.z);
        }
        
        public void AddVelocity(Vector3 newVelocity)
        {
            ClearVelocity();
            Velocity += newVelocity;
        }

        public void ClearVelocity()
        {
            Velocity = Vector3.zero;
        }

        public void ForceUnground()
        {
            motor.ForceUnground();
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
        }

        public void PostGroundingUpdate(float deltaTime)
        {
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }
}