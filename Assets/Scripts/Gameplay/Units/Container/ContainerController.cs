using Cysharp.Threading.Tasks;
using Gameplay.Unit.Item;
using ScriptableObjects.Unit.Item;
using ScriptableObjects.Unit.Item.Container;
using Unit;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay.Unit.Container
{
    public abstract class ContainerController : UnitController, IContainer
    {
        [System.Serializable]
        public class ItemData
        {
            public SO_Item SO_Item;
            public int amount;
        }
        
        [SerializeField] protected SO_Container so_Container;
        [SerializeField] protected ItemData[] items;
        
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
        
        protected async UniTask SpawnItems()
        {
            Vector3 point;
            foreach (var VARIABLE in items)
            {
                await UniTask.Yield();
                
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