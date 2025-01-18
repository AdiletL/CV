using Gameplay.Manager;
using Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Gameplay.Spawner
{
    public class DamagePopUpSpawner : MonoBehaviour, ISpawner
    {
        private IPoolableObject poolManager;

        [Inject]
        private void Construct(IPoolableObject poolManager)
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