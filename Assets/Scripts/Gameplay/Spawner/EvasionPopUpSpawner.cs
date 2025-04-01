using System;
using Gameplay.Manager;
using Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Gameplay.Spawner
{
    public class EvasionPopUpSpawner 
    {
        [Inject] private PoolManager poolManager;
        
        public async void CreatePopUp(Vector3 center)
        {
            GameObject newGameObject = null;
            newGameObject = await poolManager.GetObjectAsync<EvasionPopUp>();
            newGameObject.transform.position = center;
            newGameObject.GetComponent<EvasionPopUp>().Play();
        }
    }
}