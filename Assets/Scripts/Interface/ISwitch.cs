
using Machine;
using UnityEngine;

public interface ISwitchState
{
    public void Initialize();

    public void SetState();

    public void ExitOtherStates();

    public void ExitCategory(StateCategory category);
    public void SetTarget(GameObject target);
}
