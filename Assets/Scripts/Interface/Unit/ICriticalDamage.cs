using System.Collections.Generic;
using Calculate;
using Gameplay;

public interface ICriticalDamage : IActivatable
{
    public ValueType ValueTypeID { get; }
    public float Value { get; }
    public float ChanceValue { get; }
    
    public float GetCalculateDamage(float baseDamage);
    public bool TryApply();
}

public interface ICriticalDamageApplier
{
    public ICriticalDamage CriticalDamage { get; }
}

public interface ICriticalDamageProvider
{
    public List<float> GetCriticalDamages(float baseDamage);
}
