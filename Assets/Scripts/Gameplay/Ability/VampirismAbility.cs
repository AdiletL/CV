using Calculate;
using Gameplay.Spawner;
using Gameplay.Unit;
using Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Ability
{
    public class VampirismAbility : Ability
    {
        [Inject] private HealPopUpPopUpSpawner healPopUpPopUpSpawner;

        public override AbilityType AbilityType { get; protected set; } = AbilityType.Vampirism;

        private GameObject owner;
        private IHealth ownerHealth;
        private UnitCenter ownerUnitCenter;
        private ValueType valueType;
        private int value;
        
        public void SetOwner(GameObject owner) => this.owner = owner;
        public void SetValueType(ValueType value) => valueType = value;
        public void SetValue(int value) => this.value = value;

        public override void Initialize()
        {
            base.Initialize();
            ownerHealth = owner.GetComponent<IHealth>();
            ownerUnitCenter = owner.GetComponent<UnitCenter>();
        }

        public void Heal(float totalDamage)
        {
            var gameValue = new Calculate.GameValue(value, valueType);
            var result = gameValue.Calculate(totalDamage);
            ownerHealth.HealthStat.AddValue((int)result);
            healPopUpPopUpSpawner.CreatePopUp(ownerUnitCenter.Center.position, result);
        }
    }

    [System.Serializable]
    public class VampirismConfig : AbilityConfig
    {
        public ValueType ValueType;
        public int Value;
    }

    public class ApplyDamageHealBuilder : AbilityBuilder<VampirismAbility>
    {
        public ApplyDamageHealBuilder() : base(new VampirismAbility())
        {
        }

        public ApplyDamageHealBuilder SetValueType(ValueType valueType)
        {
            if(ability is VampirismAbility applyDamageHeal)
                applyDamageHeal.SetValueType(valueType);
            return this;
        }
        public ApplyDamageHealBuilder SetValue(int value)
        {
            if(ability is VampirismAbility applyDamageHeal)
                applyDamageHeal.SetValue(value);
            return this;
        }
        public ApplyDamageHealBuilder SetOwner(GameObject owner)
        {
            if(ability is VampirismAbility applyDamageHeal)
                applyDamageHeal.SetOwner(owner);
            return this;
        }
    }
}