using System;
using Gameplay.UI;
using UnityEngine;

namespace Gameplay.Spawner
{
    public class EvasionPopUpSpawner : PopUpSpawner
    {
        public async void CreatePopUp(Vector3 center)
        {
            GameObject newGameObject = null;
            newGameObject = await poolManager.GetObjectAsync<EvasionPopUp>();
            newGameObject.transform.position = center;
            newGameObject.GetComponent<EvasionPopUp>().Play();
        }
    }
}