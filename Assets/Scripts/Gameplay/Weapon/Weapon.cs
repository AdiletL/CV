using Calculate;
using Cysharp.Threading.Tasks;
using Unit;
using Unit.Character;
using UnityEngine;

namespace Gameplay.Weapon
{
    public abstract class Weapon : IWeapon
    {
        protected GameObject gameObject;
        protected GameObject weapon;
        protected Transform weaponParent;
        protected Transform ownerCenter;
        protected ValueType reductionEnduranceType;
        protected ValueType increaseAttackSpeedType;
        protected LayerMask enemyLayer;
        
        protected Vector3 direction;
        
        protected int characterAttackSpeed;
        protected int characterReductionEndurance;
        protected int reductionEndurance;
        protected int increaseAttackSpeed;
        protected float angleToTarget;

        public GameObject WeaponPrefab { get; protected set; }
        public GameObject CurrentTarget { get; protected set; }
        public IDamageable Damageable { get; protected set; }
        public float Range { get; protected set; }

        
        public void SetOwnerCenter(Transform ownerCenter) => this.ownerCenter = ownerCenter;
        public void SetWeaponPrefab(GameObject weapon) => WeaponPrefab = weapon;
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetDamageable(IDamageable damageable) => this.Damageable = damageable;
        public void SetAngleToTarget(float angle) => angleToTarget = angle;
        public void SetRange(float range) => this.Range = range;
        public void SetEnemyLayer(LayerMask enemyLayer) => this.enemyLayer = enemyLayer;
        

        public void SetReductionEndurance(ValueType reductionEnduranceType, int reductionEndurance)
        {
            this.reductionEnduranceType = reductionEnduranceType;
            this.reductionEndurance = reductionEndurance;
        }

        public void SetIncreaseAttackSpeed(ValueType increaseAttackSpeedType, int increaseAttackSpeed)
        {
            this.increaseAttackSpeedType = increaseAttackSpeedType;
            this.increaseAttackSpeed = increaseAttackSpeed;
        }

        public void SetWeaponParent(Transform parent)
        {
            weapon.transform.SetParent(parent);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
        }
        
        public virtual void Initialize()
        {
            weapon = Object.Instantiate(WeaponPrefab, weaponParent);
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
        public void SetDirection(Vector3 direction) => this.direction = direction;

        public abstract UniTask FireAsync();


        public virtual void ApplyDamage()
        {
            if (CurrentTarget.TryGetComponent(out IAttackable attackable) &&
                CurrentTarget.TryGetComponent(out IHealth health) &&
                health.IsLive)
            {
                attackable.TakeDamage(Damageable);
            }
        }
        
        public virtual void IncreaseStates(IState state)
        {
            
        }

        public virtual void ResetCharacterStates(CharacterWeaponAttackState characterWeaponAttackState)
        {
            Damageable.RemoveAdditionalDamage(characterWeaponAttackState.Damageable.CurrentDamage);
            characterWeaponAttackState.RemoveAttackSpeed(characterAttackSpeed);
            characterWeaponAttackState.RemoveReductionEndurance(characterReductionEndurance);
        }

        public virtual void UpdateCharacterStates(CharacterWeaponAttackState characterWeaponAttackState)
        {
            Damageable.AddAdditionalDamage(characterWeaponAttackState.Damageable.CurrentDamage);
            var attackSpeedValue = new GameValue(increaseAttackSpeed, increaseAttackSpeedType);
            
            characterAttackSpeed = attackSpeedValue.Calculate(characterWeaponAttackState.AttackSpeed);
            characterWeaponAttackState.AddAttackSpeed(characterAttackSpeed);
            
            var reductionEnduranceValue = new GameValue(reductionEndurance, reductionEnduranceType);
            characterReductionEndurance =
                reductionEnduranceValue.Calculate(characterWeaponAttackState.CurrentReductionEndurance);
            characterWeaponAttackState.AddReductionEndurance(characterReductionEndurance);
        }
    }

    public abstract class WeaponBuilder
    {
        protected Weapon weapon;

        public WeaponBuilder(Weapon weapon)
        {
            this.weapon = weapon;
        }

        public WeaponBuilder SetOwnerCenter(Transform ownerCenter)
        {
            weapon.SetOwnerCenter(ownerCenter);
            return this;
        }
        
        public WeaponBuilder SetDamageable(IDamageable damageable)
        {
            weapon.SetDamageable(damageable);
            return this;
        }

        public WeaponBuilder SetRange(float range)
        {
            weapon.SetRange(range);
            return this;
        }
        public WeaponBuilder SetWeaponPrefab(GameObject prefab)
        {
            weapon.SetWeaponPrefab(prefab);
            return this;
        }
        
        public WeaponBuilder SetGameObject(GameObject gameObject)
        {
            weapon.SetGameObject(gameObject);
            return this;
        }
        public WeaponBuilder SetReductionEndurance(ValueType valueType, int value)
        {
            weapon.SetReductionEndurance(valueType, value);
            return this;
        }
        public WeaponBuilder SetAngleToTarget(float value)
        {
            weapon.SetAngleToTarget(value);
            return this;
        }
        public WeaponBuilder SetIncreaseAttackSpeed(ValueType valueType, int amount)
        {
            weapon.SetIncreaseAttackSpeed(valueType, amount);
            return this;
        }
        public Weapon Build()
        {
            return weapon;
        }
    }
}