using Cysharp.Threading.Tasks;
using Unit.Trap;
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

        private void SpawnTraps()
        {
            foreach (var item in trapPrefabs)
            {
                var platform = platformSpawner.GetFreePlatform();
                if(!platform) return;
                
                var newGameObject = diContainer.InstantiatePrefabForComponent<TrapController>(item);
                newGameObject.transform.position = platform.transform.position;
                var tower = newGameObject.GetComponent<TrapController>();
                diContainer.Inject(tower);
                tower.Initialize();
            }
        }
    }
}