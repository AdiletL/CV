using Gameplay.Unit;
using UnityEngine;

public interface IClickableObject : IInteractable
{
    public void ShowInformation();
    public void UpdateInformation();
    public void HideInformation();
}