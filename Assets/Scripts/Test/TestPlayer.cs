using System;
using UnityEngine;

namespace Test
{
    [RequireComponent(typeof(CharacterController))]
    public class TestPlayer : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 2;
        [SerializeField] private float jumpPower = 3;
        
        private CharacterController characterController;
        private TestResource currentCollisionResource;
        private Vector3 directionMovement;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            CheckInput();
            ExecuteMovement();
        }

        private void CheckInput()
        {
            directionMovement.z = 0;
            
            if (Input.GetKey(KeyCode.A)) directionMovement.z = -1;
            if (Input.GetKey(KeyCode.D)) directionMovement.z = 1;
            if (Input.GetKey(KeyCode.W)) directionMovement.y = 1;
            if (Input.GetKey(KeyCode.S)) directionMovement.y = -1;
            if (Input.GetKeyDown(KeyCode.Space)) ExecuteJump();
            if (Input.GetKeyDown(KeyCode.F)) ExecuteInteraction();
        }

        private void ExecuteMovement()
        {
            characterController.SimpleMove(directionMovement * (movementSpeed * Time.deltaTime));
        }

        private void ExecuteJump()
        {
            if(characterController.isGrounded)
                directionMovement.y = jumpPower;
        }

        private void ExecuteInteraction()
        {
            Debug.Log("ExecuteInteraction");
        }

        private void SetTargetResource(TestResource targetResource)
        {
            if(currentCollisionResource != null && currentCollisionResource != targetResource)
                currentCollisionResource = targetResource;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out TestResource testResource))
            {
                SetTargetResource(testResource);
            }
        }
    }
}