using Cysharp.Threading.Tasks;
using Gameplay.Manager;
using Gameplay.UI;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Gameplay.Spawner
{
    public class DamagePopUpSpawner : MonoBehaviour, ISpawner
    {
        [Inject] private PoolManager poolManager;
        
        [PunRPC]
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