using Gameplay.Manager;
using UnityEngine;
using Zenject;

namespace Gameplay.Spawner
{
    public abstract class PopUpSpawner : MonoBehaviour, ISpawner
    {
        [Inject] protected PoolManager poolManager;

        public abstract void Initialize();

        public abstract void CreatePopUp(Vector3 center, int damage);
    }
}