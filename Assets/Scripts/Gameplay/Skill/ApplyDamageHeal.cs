using Calculate;

namespace Gameplay.Skill
{
    public class ApplyDamageHeal : Skill
    {
        public override SkillType SkillType { get; protected set; } = SkillType.applyDamageHeal;
        
        public ValueType ValueType { get; private set; }
        public int Value { get; private set; }
        
        
        public void SetValueType(ValueType value) => ValueType = value;
        public void SetValue(int value) => Value = value;
    }

    [System.Serializable]
    public class ApplyDamageHealConfig : SkillConfig
    {
        public ValueType ValueType;
        public int Value;
    }

    public class ApplyDamageHealBuilder : SkillBuilder<ApplyDamageHeal>
    {
        public ApplyDamageHealBuilder() : base(new ApplyDamageHeal())
        {
        }

        public ApplyDamageHealBuilder SetValueType(ValueType valueType)
        {
            if(skill is ApplyDamageHeal applyDamageHeal)
                applyDamageHeal.SetValueType(valueType);
            return this;
        }
        public ApplyDamageHealBuilder SetValue(int value)
        {
            if(skill is ApplyDamageHeal applyDamageHeal)
                applyDamageHeal.SetValue(value);
            return this;
        }
    }
}