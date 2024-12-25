using Cysharp.Threading.Tasks;
using Unit.Trap;
using Unit.Trap.Activator;
using UnityEngine;
using Zenject;

namespace Gameplay.Spawner
{
    public class TrapSpawner : MonoBehaviour, ISpawner
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private GameObject[] trapPrefabs;
        
        private PlatformSpawner platformSpawner;
        
        public async void Initialize()
        {
            await UniTask.WaitForEndOfFrame();
        }

        public async UniTask Execute()
        {
            SpawnTraps();

            await UniTask.WaitForEndOfFrame();
        }
        
        public void SetSpawners(PlatformSpawner platformSpawner)
        {
            this.platformSpawner = platformSpawner;
        }

        private ThornController thornController;
        
        private void ButtonActivator(ButtonController buttonActivator)
        {
            buttonActivator.AddTrap(thornController);
        }
        private void SpawnTraps()
        {
            foreach (var item in trapPrefabs)
            {
                var platform = platformSpawner.GetFreePlatform();
                if(!platform) return;
                
                var newGameObject = diContainer.InstantiatePrefabForComponent<TrapController>(item);
                newGameObject.transform.position = platform.transform.position;
                var trap = newGameObject.GetComponent<TrapController>();
                diContainer.Inject(trap);
                trap.Initialize();
                
                if(trap is ThornController thornController)
                    this.thornController = thornController;
                if(trap is ButtonController buttonController)
                    this.ButtonActivator(buttonController);
            }
        }
    }
}