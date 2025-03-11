using Photon.Pun;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public abstract class TrapController : UnitController, ITrap
    {
        [SerializeField] protected SO_Trap so_Trap;

        protected PhotonView photonView;
        protected TrapAnimation trapAnimation;
        protected AnimationClip appearClip;
        protected AnimationClip deappearClip;

        public GameObject CurrentTarget { get; protected set; }
        public LayerMask EnemyLayer { get; protected set; }
        
        public override void Initialize()
        {
            base.Initialize();
            appearClip = so_Trap.AppearClip;
            deappearClip = so_Trap.DeappearClip;
            EnemyLayer = so_Trap.EnemyLayer;
            trapAnimation = GetComponentInUnit<TrapAnimation>();
            trapAnimation.Initialize();
            trapAnimation.AddClip(appearClip);
            trapAnimation.AddClip(deappearClip);
        }
        

        public abstract void Trigger();
        public abstract void Reset();

        
        public void SetTarget(GameObject target) => CurrentTarget = target;
    }
}