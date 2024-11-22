using Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Weapon
{
    public abstract class Weapon : IWeapon
    {
        protected GameObject weapon;
        
        public GameObject WeaponPrefab { get; set; }
        public Transform WeaponParent { get; set; }
        public GameObject CurrentTarget { get; private set; }
        public IDamageable Damageable { get; set; }
        public int AmountAttack { get; set; }
        public float Range { get; set; }
        
        
        public virtual void Initialize()
        {
            weapon = Object.Instantiate(WeaponPrefab, WeaponParent);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            Hide(); 
        }
        

        public virtual void IncreaseStates(IState state)
        {
            
        }

        public void SetTarget(GameObject target)
        {
            CurrentTarget = target;
        }

        public virtual void ApplyDamage()
        {
            
        }

        public void Show() => weapon.SetActive(true);

        public void Hide() => weapon.SetActive(false);
    }

    public abstract class WeaponBuilder
    {
        private Weapon weapon;

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


        public WeaponBuilder SetAmountAttack(int amount)
        {
            weapon.AmountAttack = amount;
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
        public Weapon Build()
        {
            return weapon;
        }
    }
}