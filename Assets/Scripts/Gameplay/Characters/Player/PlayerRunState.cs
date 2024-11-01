using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerRunState : CharacterRunState
    {
        private GameObject currentTarget;
        private Quaternion targetForRotate;
        

        public override void Update()
        {
            Move();
        }

        public void SetTarget(GameObject target)
        {
            currentTarget = target;
        }

        public override void Move()
        {
            GameObject.transform.position = Vector3.MoveTowards(GameObject.transform.position, currentTarget.transform.position, MovementSpeed * Time.deltaTime);
        }
        
    }

    public class PlayerBaseRunStateBuilder : CharacterBaseRunStateBuilder
    {
        public PlayerBaseRunStateBuilder() : base(new PlayerRunState())
        {
        }
    }
}