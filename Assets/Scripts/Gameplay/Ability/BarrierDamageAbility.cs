using Calculate;
using UnityEngine;

namespace Gameplay.Ability
{
    public class BarrierDamageAbility : Ability
    {
        public override AbilityType AbilityTypeID { get; protected set; } = AbilityType.BarrierDamage;

        private DamageType damageType;
        private GameValue gameValue;
        private float amount;
        
        public void SetDamageType(DamageType damageType) => this.damageType = damageType;
        public void SetGameValue(ValueType valueType, float value) => gameValue = new GameValue(value, valueType);
        public void SetAmount(float amount) => this.amount = amount;
        

        public DamageData DamageModify(DamageData damageData)
        {
            if (damageData.DamageTypeID.HasFlag(damageType))
            {
                amount = gameValue.Calculate(damageData.Amount);
                if(amount > 0) damageData.Amount = 0;
                else damageData.Amount = Mathf.Abs(amount);
            }
            return damageData;
        }
    }
    
    public class BarrierDamageAbilityBuilder : AbilityBuilder
    {
        public BarrierDamageAbilityBuilder() : base(new BarrierDamageAbility())
        {
        }

        public BarrierDamageAbilityBuilder SetDamageType(DamageType damageType)
        {
            if(ability is BarrierDamageAbility barrierDamage)
                barrierDamage.SetDamageType(damageType);
            return this;
        }
        public BarrierDamageAbilityBuilder SetGameValue(ValueType valueType, float value)
        {
            if(ability is BarrierDamageAbility barrierDamage)
                barrierDamage.SetGameValue(valueType, value);
            return this;
        }
        public BarrierDamageAbilityBuilder SetAmount(float amount)
        {
            if(ability is BarrierDamageAbility barrierDamage)
                barrierDamage.SetAmount(amount);
            return this;
        }
    }
}