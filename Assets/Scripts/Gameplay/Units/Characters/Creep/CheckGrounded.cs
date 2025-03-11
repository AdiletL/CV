using System;
using UnityEngine;

namespace Gameplay.Unit.Character.Creep
{
    public class CheckGrounded : MonoBehaviour
    {
        private float lenght;
        private int baseLayer;
        
        public bool IsGrounded()
        {
            Debug.DrawRay(transform.position, Vector3.down * lenght, Color.green);
            if (Physics.Raycast(transform.position, Vector3.down, lenght, ~baseLayer))
                return true;
            return false;
        }

        private void Start()
        {
            lenght = transform.position.y + .1f;
            baseLayer = gameObject.layer;
        }
    }
}