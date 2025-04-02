using Gameplay.UI;
using UnityEngine;

namespace Gameplay.Spawner
{
    public class HealPopUpSpawner : PopUpSpawner
    {
        public async void CreatePopUp(Vector3 center, float damage)
        {
            var popUpGameObject = await poolManager.GetObjectAsync<HealPopUpUI>();
            popUpGameObject.transform.position = center;
            popUpGameObject.GetComponent<HealPopUpUI>().Play(damage);
        }
    }
}