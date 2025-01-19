using Cysharp.Threading.Tasks;
using Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Gameplay.Spawner
{
    public class DamagePopUpSpawner : MonoBehaviour, ISpawner
    {
        [Inject] private IPoolableObject poolManager;
        
        public async UniTask Initialize()
        {
            await UniTask.CompletedTask;
        }

        public async void CreatePopUp(Vector3 center, int damage)
        {
            var popUpGameObject = await poolManager.GetObjectAsync<DamageUI>();
            popUpGameObject.transform.position = center;
            popUpGameObject.GetComponent<DamageUI>().Play(damage);
        }
    }
}