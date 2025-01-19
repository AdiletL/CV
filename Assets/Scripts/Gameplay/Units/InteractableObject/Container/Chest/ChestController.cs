using UnityEngine;

namespace Unit.InteractableObject.Container
{
    public class ChestController : ContainerController
    {
        private bool isInteractable;
        
        public override void Appear()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Open();
            }
        }
        
        public override void Open()
        {
            
        }

        public override void Close()
        {
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(Layers.PLAYER_LAYER, other.gameObject.layer))
                return;

            isInteractable = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(Layers.PLAYER_LAYER, other.gameObject.layer))
                return;

            isInteractable = false;
        }
    }
}
