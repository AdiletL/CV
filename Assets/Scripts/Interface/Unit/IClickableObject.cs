using Unit;
using UnityEngine;

public interface IClickableObject : IInteractable
{
    public GameObject SelectedObjectVisual { get; }
    public UnitInformation UnitInformation { get; }
    public void ShowInformation();
    public void UpdateInformation();
    public void HideInformation();
    public void SelectObject();
    public void UnSelectObject();
}