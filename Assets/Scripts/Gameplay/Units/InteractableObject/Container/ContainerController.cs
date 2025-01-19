using ScriptableObjects.Unit.InteractableObject.Container;
using UnityEngine;

namespace Unit.InteractableObject.Container
{
    public abstract class ContainerController : InteractableObjectController
    {
        [SerializeField] protected SO_Container so_Container;

        protected AnimationClip openClip;
        protected AnimationClip closeClip;
        protected bool isOpened;

        public override void Initialize()
        {
            base.Initialize();
            openClip = so_Container.OpenClip;
            closeClip = so_Container.CloseClip;
        }

        public abstract void Open();
        public abstract void Close();
    }
}