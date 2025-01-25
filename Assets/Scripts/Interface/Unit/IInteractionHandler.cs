using UnityEngine;

namespace Unit
{
    public interface IInteractionHandler : IInputHandler
    {
        public void CheckTriggerEnter(GameObject other);
        public void CheckTriggerExit(GameObject other);
    }
}