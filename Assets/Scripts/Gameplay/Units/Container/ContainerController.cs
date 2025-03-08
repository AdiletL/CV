using ScriptableObjects.Unit.Item.Container;
using Unit;
using UnityEngine;

namespace Gameplay.Unit.Container
{
    public abstract class ContainerController : UnitController, IContainer
    {
        [SerializeField] protected SO_Container so_Container;

        protected ContainerAnimation containerAnimation;
        protected AnimationClip openClip;
        protected AnimationClip closeClip;
        protected bool isOpened;

        public override void Initialize()
        {
            base.Initialize();
            containerAnimation = GetComponentInUnit<ContainerAnimation>();
            containerAnimation.Initialize();
            openClip = so_Container.OpenClip;
            closeClip = so_Container.CloseClip;
            containerAnimation.AddClip(openClip);
            containerAnimation.AddClip(closeClip);
        }

        public abstract void Open();
        public abstract void Close();

        public abstract void Enable(KeyCode keyCode);
        public abstract void Disable();
    }
}