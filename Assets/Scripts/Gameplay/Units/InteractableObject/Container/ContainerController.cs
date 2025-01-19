namespace Unit.InteractableObject.Container
{
    public abstract class ContainerController : InteractableObjectController
    {
        protected bool isOpened;
        public abstract void Open();
        public abstract void Close();
    }
}