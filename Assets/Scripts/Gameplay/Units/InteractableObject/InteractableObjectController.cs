using UnityEngine;

namespace Unit.InteractableObject
{
    public abstract class InteractableObjectController : UnitController, IClickableObject
    {
        public UnitInformation UnitInformation { get; }
        public void ShowInformation()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateInformation()
        {
            throw new System.NotImplementedException();
        }

        public void HideInformation()
        {
            throw new System.NotImplementedException();
        }
    }
}