using System;
using Gameplay.Effect;

public enum DisableType
{
    Nothing,
    Stun,
    Root,
}

public interface IDisable
{
    public event Action<float, float> OnCountTimer;
    public event Action<IDisable> OnDestroyDisable;
    public DisableType DisableTypeID { get; }
    public float Timer { get; }
    public float CountTimer { get; }
}

public interface IDisablelable
{
    public DisableType CurrentDisableType { get; }
    public void Initialize();
    public void SetDisableType(DisableType disableType);
    public void ActivateDisable(DisableType disableType);
    public void DeactivateDisable(DisableType disableType);
    public void CleanseDisable();
}

public interface IDisableController
{
    public void Initialize();
    public void ActivateDisable(IDisable disableEffect);
    public void DeactivateDisable(IDisable disableEffect);
}

