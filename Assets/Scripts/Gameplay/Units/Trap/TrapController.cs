using ScriptableObjects.Gameplay.Trap;
using UnityEngine;

namespace Unit.Trap
{
    public abstract class TrapController : UnitController, ITrap
    {
        public override UnitType UnitType { get; } = UnitType.trap;

        [SerializeField] protected SO_Trap so_Trap;


        protected AnimationClip activateClip;
        protected AnimationClip deactivateClip;

        public GameObject CurrentTarget { get; protected set; }
        public LayerMask[] EnemyLayers { get; protected set; }
        
        
        public override T GetComponentInUnit<T>()
        {
            return components.GetComponentFromArray<T>();
        }
        

        public override void Initialize()
        {
            base.Initialize();
            activateClip = so_Trap.ActivateClip;
            deactivateClip = so_Trap.DeactivateClip;
            EnemyLayers = so_Trap.EnemyLayers;
        }

        public abstract void Activate();
        public abstract void Deactivate();

        
        public void SetTarget(GameObject target)
        {
            CurrentTarget = target;
        }
    }
}