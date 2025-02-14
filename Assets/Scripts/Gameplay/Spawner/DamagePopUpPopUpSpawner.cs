using Cysharp.Threading.Tasks;
using Gameplay.UI;
using Photon.Pun;
using UnityEngine;

namespace Gameplay.Spawner
{
    public class DamagePopUpPopUpSpawner : PopUpSpawner
    {
        
        [PunRPC]
        public override void Initialize()
        {
            
        }

        public override async void CreatePopUp(Vector3 center, int damage)
        {
            var popUpGameObject = await poolManager.GetObjectAsync<DamagePopUpUI>();
            popUpGameObject.transform.position = center;
            popUpGameObject.GetComponent<DamagePopUpUI>().Play(damage);
        }
    }
}