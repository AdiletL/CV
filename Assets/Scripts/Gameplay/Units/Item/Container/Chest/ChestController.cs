using Cysharp.Threading.Tasks;
using Gameplay.UI;
using ScriptableObjects.Unit.Item.Container;
using Unit.Item.Loot;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Unit.Item.Container
{
    public class ChestController : ContainerController
    {
        [SerializeField] private AssetReference[] lootPrefabs;
        
        private SO_Chest so_chest;
        private HotkeyUI hotkeyUI;
        private ChestAnimation chestAnimation;
        
        private bool isInteractable;

        public override void Initialize()
        {
            base.Initialize();

            so_chest = (SO_Chest)so_Container;
            chestAnimation = (ChestAnimation)containerAnimation;
            hotkeyUI = GetComponentInUnit<HotkeyUI>();
            hotkeyUI.Hide();
        }

        public override void Appear()
        {
            
        }
        
        public override void Open()
        {
            isOpened = true;
            chestAnimation.ChangeAnimationWithSpeed(openClip);
            Disable();
            SpawnLoot();
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

        private async UniTask SpawnLoot()
        {
            Vector3 point;
            foreach (var VARIABLE in lootPrefabs)
            {
                var loot = await Addressables.InstantiateAsync(VARIABLE);
                var lootController = loot.GetComponent<LootController>();
                diContainer.Inject(lootController);
                lootController.Initialize();
                loot.transform.position = transform.position;
                Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(0.5f, 1f));
                point = transform.position + transform.right * randomOffset.x + transform.forward * randomOffset.z;
                lootController.JumpToPoint(point);
            }
        }
    }
}
