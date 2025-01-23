using Calculate;
using Cysharp.Threading.Tasks;
using Unit;
using Unit.Character;
using UnityEngine;

namespace Gameplay.Weapon
{
    public abstract class Weapon : IWeapon
    {
        protected GameObject weapon;
        
        protected int characterAttackSpeed;
        protected int characterReductionEndurance;
        
        public GameObject WeaponPrefab { get; set; }
        public Transform WeaponParent { get; set; }
        public GameObject GameObject { get; set; }
        public GameObject CurrentTarget { get; private set; }
        public IDamageable Damageable { get; set; }
        public float AngleToTarget { get; set; }
        public float Range { get; set; }
        public ValueType ReductionEnduranceType { get; set; }
        public int ReductionEndurance { get; set; }
        public ValueType IncreaseAttackSpeedType { get; set; }
        public int IncreaseAttackSpeed { get; set; }

        
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
            var attackSpeedValue = new GameValue(IncreaseAttackSpeed, IncreaseAttackSpeedType);
            characterAttackSpeed = attackSpeedValue.Calculate(characterWeaponAttackState.AttackSpeed);
            characterWeaponAttackState.AddAttackSpeed(characterAttackSpeed);
            var reductionEnduranceValue = new GameValue(ReductionEndurance, ReductionEnduranceType);
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
        public WeaponBuilder SetReductionEndurance(ValueType valueType, int value)
        {
            weapon.ReductionEnduranceType = valueType;
            weapon.ReductionEndurance = value;
            return this;
        }
        public WeaponBuilder SetAngleToTarget(float value)
        {
            weapon.AngleToTarget = value;
            return this;
        }
        public WeaponBuilder SetIncreaseAttackSpeed(ValueType valueType, int amount)
        {
            weapon.IncreaseAttackSpeedType = valueType;
            weapon.IncreaseAttackSpeed = amount;
            return this;
        }
        public Weapon Build()
        {
            return weapon;
        }
    }
}