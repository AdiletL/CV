using Calculate;

namespace Gameplay.Ability
{
    public class ApplyDamageHeal : Ability
    {
        public override AbilityType AbilityType { get; protected set; } = AbilityType.ApplyDamageHeal;
        
        public ValueType ValueType { get; private set; }
        public int Value { get; private set; }
        
        public void SetValueType(ValueType value) => ValueType = value;
        public void SetValue(int value) => Value = value;
    }

    [System.Serializable]
    public class ApplyDamageHealConfig : AbilityConfig
    {
        public ValueType ValueType;
        public int Value;
    }

    public class ApplyDamageHealBuilder : AbilityBuilder<ApplyDamageHeal>
    {
        public ApplyDamageHealBuilder() : base(new ApplyDamageHeal())
        {
        }

        public ApplyDamageHealBuilder SetValueType(ValueType valueType)
        {
            if(ability is ApplyDamageHeal applyDamageHeal)
                applyDamageHeal.SetValueType(valueType);
            return this;
        }
        public ApplyDamageHealBuilder SetValue(int value)
        {
            if(ability is ApplyDamageHeal applyDamageHeal)
                applyDamageHeal.SetValue(value);
            return this;
        }
    }
}