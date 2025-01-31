using Photon.Pun;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;

namespace Unit.Trap
{
    public abstract class TrapController : UnitController, ITrap
    {
        [SerializeField] protected SO_Trap so_Trap;

        private PhotonView photonView;

        protected AnimationClip activateClip;
        protected AnimationClip deactivateClip;

        public GameObject CurrentTarget { get; protected set; }
        public LayerMask EnemyLayer { get; protected set; }
        
        public override void Initialize()
        {
            base.Initialize();
            activateClip = so_Trap.ActivateClip;
            deactivateClip = so_Trap.DeactivateClip;
            EnemyLayer = so_Trap.EnemyLayer;
        }
        

        public abstract void Activate();
        public abstract void Deactivate();

        
        public void SetTarget(GameObject target)
        {
            CurrentTarget = target;
        }
    }
}