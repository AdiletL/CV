using ScriptableObjects.Gameplay.Trap;
using UnityEngine;

namespace Unit.Trap.Fall
{
    public class FallGravityController : TrapController, IFallGravity
    {
        private SO_FallGravity so_FallGravity;
        
        public Rigidbody Rigidbody { get; private set; }
        public float Mass { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            
            
        }
        
        public override void Appear()
        {
            
        }

        public override void Activate()
        {
            throw new System.NotImplementedException();
        }

        public override void Deactivate()
        {
            throw new System.NotImplementedException();
        }

        public void FallGravity()
        {
            throw new System.NotImplementedException();
        }
    }
}