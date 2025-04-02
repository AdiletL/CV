using Gameplay.UI;
using UnityEngine;

namespace Gameplay.Spawner
{
    public class CriticalDamagePopUpSpawner : PopUpSpawner
    {
        public async void CreatePopUp(Vector3 center, float value)
        {
            GameObject newGameObject = null;
            newGameObject = await poolManager.GetObjectAsync<CriticalDamagePopUpUI>();
            if(!newGameObject) return;
            
            newGameObject.transform.position = center;
            newGameObject.GetComponent<PopUpUI>().Play(value);
        }
    }
}