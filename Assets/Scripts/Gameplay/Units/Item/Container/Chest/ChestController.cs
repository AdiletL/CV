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
        
        private bool isInteractable;

        public override void Initialize()
        {
            base.Initialize();

            so_chest = (SO_Chest)so_Container;

            hotkeyUI = GetComponentInUnit<HotkeyUI>();
            hotkeyUI.Hide();
        }

        public override void Appear()
        {
            
        }
        
        public override void Open()
        {
            isOpened = true;
            GetComponentInUnit<ChestAnimation>().ChangeAnimationWithSpeed(openClip);
            Disable();
            SpawnLoot();
        }

        public override void Close()
        {
            GetComponentInUnit<ChestAnimation>().ChangeAnimationWithSpeed(closeClip);
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
            float randomX;
            float randomZ;
            foreach (var VARIABLE in lootPrefabs)
            {
                var loot = await Addressables.InstantiateAsync(VARIABLE);
                var lootController = loot.GetComponent<LootController>();
                diContainer.Inject(lootController);
                lootController.Initialize();
                loot.transform.position = transform.position;
                randomX = Random.Range(-1, 1);
                randomZ = Random.Range(1, 2);
                point = transform.forward + new Vector3(transform.position.x + randomX, transform.position.y, randomZ);
                lootController.JumpToPoint(point);
            }
        }
    }
}
