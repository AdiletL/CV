using Calculate;
using Gameplay.Spawner;
using Gameplay.Unit;
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
        private GameValue gameValue;
        
        public void SetOwner(GameObject owner) => this.owner = owner;
        public void SetGameValue(ValueType valueType, float value) => gameValue = new GameValue(value, valueType);

        public override void Initialize()
        {
            base.Initialize();
            ownerHealth = owner.GetComponent<IHealth>();
            ownerUnitCenter = owner.GetComponent<UnitCenter>();
        }

        public void Heal(float totalDamage)
        {
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

    public class ApplyDamageHealBuilder : AbilityBuilder
    {
        public ApplyDamageHealBuilder() : base(new VampirismAbility())
        {
        }

        public ApplyDamageHealBuilder SetGameValue(ValueType valueType, float value)
        {
            if(ability is VampirismAbility applyDamageHeal)
                applyDamageHeal.SetGameValue(valueType, value);
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