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
        
        public BarrierDamageAbility(AbilityConfig abilityConfig) : base(abilityConfig)
        {
            
        }
        
        
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
}