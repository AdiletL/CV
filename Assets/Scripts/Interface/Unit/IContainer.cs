using UnityEngine;

namespace Unit
{
    public interface IContainer
    {
        public void Open();
        public void Close();
        public void Enable(KeyCode openKey);
        public void Disable();
    }
}