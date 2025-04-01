using Gameplay.Manager;
using Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Gameplay.Spawner
{
    public class HealPopUpSpawner
    {
        [Inject] private PoolManager poolManager;
        
        public async void CreatePopUp(Vector3 center, float damage)
        {
            var popUpGameObject = await poolManager.GetObjectAsync<HealPopUpUI>();
            popUpGameObject.transform.position = center;
            popUpGameObject.GetComponent<HealPopUpUI>().Play(damage);
        }
    }
}