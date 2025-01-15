using Cysharp.Threading.Tasks;
using Unit;
using UnityEngine;

namespace Gameplay.Weapon
{
    public abstract class Weapon : IWeapon
    {
        protected GameObject weapon;
        
        public GameObject WeaponPrefab { get; set; }
        public Transform WeaponParent { get; set; }
        public GameObject GameObject { get; set; }
        public GameObject CurrentTarget { get; private set; }
        public IDamageable Damageable { get; set; }
        public float Range { get; set; }
        public float DecreaseEndurance { get; set; }
        
        
        public virtual void Initialize()
        {
            weapon = Object.Instantiate(WeaponPrefab, WeaponParent);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            Hide(); 
        }
        
        public void Show() => weapon.SetActive(true);

        public void Hide() => weapon.SetActive(false);
        
        public void SetTarget(GameObject target)
        {
            CurrentTarget = target;
        }

        public abstract UniTask FireAsync();

        public virtual void ApplyDamage()
        {
            if (CurrentTarget.TryGetComponent(out IHealth health)
                && health.IsLive)
            {
                health.TakeDamage(Damageable);
            }
        }
        
        public virtual void IncreaseStates(IState state)
        {
            
        }

    }

    public abstract class WeaponBuilder
    {
        protected Weapon weapon;

        public WeaponBuilder(Weapon weapon)
        {
            this.weapon = weapon;
        }

        public WeaponBuilder SetDamageable(IDamageable damageable)
        {
            weapon.Damageable = damageable;
            return this;
        }

        public WeaponBuilder SetRange(float range)
        {
            weapon.Range = range;
            return this;
        }
        public WeaponBuilder SetWeaponPrefab(GameObject prefab)
        {
            weapon.WeaponPrefab = prefab;
            return this;
        }

        public WeaponBuilder SetWeaponParent(Transform parent)
        {
            weapon.WeaponParent = parent;
            return this;
        }
        public WeaponBuilder SetGameObject(GameObject gameObject)
        {
            weapon.GameObject = gameObject;
            return this;
        }
        public WeaponBuilder SetDecreaseEndurance(float value)
        {
            weapon.DecreaseEndurance = value;
            return this;
        }
        public Weapon Build()
        {
            return weapon;
        }
    }
}