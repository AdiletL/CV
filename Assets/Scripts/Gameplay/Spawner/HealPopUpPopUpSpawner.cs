using Gameplay.UI;
using UnityEngine;

namespace Gameplay.Spawner
{
    public class HealPopUpPopUpSpawner : PopUpSpawner
    {
        public override void Initialize()
        {
            
        }

        public override async void CreatePopUp(Vector3 center, int damage)
        {
            var popUpGameObject = await poolManager.GetObjectAsync<HealPopUpUI>();
            popUpGameObject.transform.position = center;
            popUpGameObject.GetComponent<HealPopUpUI>().Play(damage);
        }
    }
}