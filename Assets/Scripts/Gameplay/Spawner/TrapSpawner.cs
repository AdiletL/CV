using Cysharp.Threading.Tasks;
using Gameplay.Factory;
using Unit.Trap;
using Unit.Trap.Activator;
using UnityEngine;
using Zenject;

namespace Gameplay.Spawner
{
    public class TrapSpawner : MonoBehaviour, ISpawner
    {
        [Inject] private DiContainer diContainer;

        private TrapController[] traps;
        private PlatformSpawner platformSpawner;
        private TrapFactoryMethod trapFactoryMethod;
        
        public async void Initialize()
        {
            trapFactoryMethod = new TrapFactoryMethod();
            diContainer.Inject(trapFactoryMethod);
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
            foreach (var item in traps)
            {
                var platform = platformSpawner.GetFreePlace();
                if(!platform) return;
                
                var newGameObject = trapFactoryMethod.Create(item.GetType());
                newGameObject.transform.position = platform.transform.position;
                var trap = newGameObject.GetComponent<TrapController>();
                diContainer.Inject(trap);
                trap.Initialize();
            }
        }
    }
}