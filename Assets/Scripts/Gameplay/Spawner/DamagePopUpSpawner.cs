using Gameplay.Manager;
using Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Gameplay.Spawner
{
    public class DamagePopUpSpawner : MonoBehaviour, ISpawner
    {
        private IPoolable poolManager;

        [Inject]
        private void Construct(IPoolable poolManager)
        {
            this.poolManager = poolManager;
        }
        
        public void Initialize()
        {
        }

        public async void CreatePopUp(Vector3 center, int damage)
        {
            var popUpGameObject = await poolManager.GetObjectAsync<DamageUI>();
            popUpGameObject.transform.position = center;
            popUpGameObject.GetComponent<DamageUI>().Play(damage);
        }
    }
}