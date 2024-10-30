using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class PlayerRunState : CharacterRunState
    {
        private GameObject currentTarget;
        private Vector3 directionRotate;
        private Quaternion targetForRotate;
        

        public override void Update()
        {
            Move();
            Rotate();
        }

        public void SetTarget(GameObject target)
        {
            currentTarget = target;
        }

        public override void Move()
        {
            if (GameObject.transform.rotation == targetForRotate)
            {
                GameObject.transform.position = Vector3.MoveTowards(GameObject.transform.position, currentTarget.transform.position, MovementSpeed * Time.deltaTime);
            }
        }

        public override void Rotate()
        {
            directionRotate = currentTarget.transform.position - GameObject.transform.position;
            if (directionRotate == Vector3.zero) return;
            
            targetForRotate = Quaternion.LookRotation(directionRotate, Vector3.up);
            GameObject.transform.rotation = Quaternion.RotateTowards(GameObject.transform.rotation, targetForRotate, RotationSpeed * Time.deltaTime);
        }
    }

    public class PlayerRunStateBuilder : CharacterRunStateBuilder
    {
        public PlayerRunStateBuilder() : base(new PlayerRunState())
        {
        }
    }
}