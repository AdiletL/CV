using Cysharp.Threading.Tasks;
using Gameplay.UI;
using ScriptableObjects.Unit.Item.Container;
using Unit.Item;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Unit.Container
{
    public class ChestController : ContainerController
    {
        private SO_Chest so_Chest;
        private HotkeyUI hotkeyUI;
        private ChestAnimation chestAnimation;
        
        private bool isInteractable;

        public override void Initialize()
        {
            base.Initialize();

            so_Chest = (SO_Chest)so_Container;
            chestAnimation = (ChestAnimation)containerAnimation;
            hotkeyUI = GetComponentInUnit<HotkeyUI>();
            hotkeyUI.Hide();
        }

        public override void Appear()
        {
            
        }

        public override void Disappear()
        {
            throw new System.NotImplementedException();
        }

        public override void Open()
        {
            if(isOpened) return;
            isOpened = true;
            chestAnimation.ChangeAnimationWithSpeed(openClip);
            Disable();
            SpawnItems();
        }

        public override void Close()
        {
            chestAnimation.ChangeAnimationWithSpeed(closeClip);
            isOpened = false;
        }

        public override void Enable(KeyCode openKey)
        {
            hotkeyUI.SetText(openKey.ToString());
            if(isOpened) return;
            
            hotkeyUI.Show();
        }

        public override void Disable()
        {
            hotkeyUI.Hide();
        }

        private async UniTask SpawnItems()
        {
            Vector3 point;
            foreach (var VARIABLE in so_Chest.so_Item)
            {
                var item = await Addressables.InstantiateAsync(VARIABLE.SO_Item.Prefab);
                var itemController = item.GetComponent<ItemController>();
                diContainer.Inject(itemController);
                itemController.SetAmount(VARIABLE.amount);
                itemController.Initialize();
                item.transform.position = transform.position;
                Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(0.5f, 1f));
                point = transform.position + transform.right * randomOffset.x + transform.forward * randomOffset.z;
                itemController.JumpToPoint(point);
            }
        }
    }
}
