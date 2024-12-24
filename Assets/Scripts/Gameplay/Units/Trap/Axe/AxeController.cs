using System;
using Gameplay.Damage;
using UnityEngine;

namespace Unit.Trap
{
    public class AxeController : TrapController, IApplyDamage
    {
        [SerializeField] private Animator animator;

        private GameObject target;
        
        private const string ACTIVATE_NAME = "Activate";
        private const string DEACTIVATE_NAME = "Activate";
        
        public IDamageable Damageable { get; private set; }
        
        public override T GetComponentInUnit<T>()
        {
            return components.GetComponentFromArray<T>();
        }

        public override void Initialize()
        {
            base.Initialize();
            
            Damageable = new NormalDamage(so_Trap.Damage, gameObject);
            
            Activate();
        }

        public override void Activate()
        {
            animator.SetTrigger(ACTIVATE_NAME);
        }

        public override void Deactivate()
        {
            animator.SetTrigger(DEACTIVATE_NAME);
        }

        private void OnEnable()
        {
            GetComponentInUnit<AxeCollision>().OnHit += OnHit;
        }
        private void OnDisable()
        {
            GetComponentInUnit<AxeCollision>().OnHit -= OnHit;
        }

        private void OnHit(GameObject target)
        {
            this.target = target;
            ApplyDamage();
        }


        public void ApplyDamage()
        {
            if (target.TryGetComponent(out IHealth health) && health.IsLive)
            {
                health.TakeDamage(Damageable);
            }
        }
    }
}
