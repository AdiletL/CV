public enum DisableType
{
    Nothing,
    Stun,
    Root,
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
    public void ActivateDisable(DisableType disableType);
    public void DeactivateDisable(DisableType disableType);
}

