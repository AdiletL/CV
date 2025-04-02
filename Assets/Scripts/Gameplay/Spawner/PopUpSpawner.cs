using Gameplay.Manager;
using Zenject;

namespace Gameplay.Spawner
{
    public abstract class PopUpSpawner
    {
        [Inject] protected PoolManager poolManager;
    }
}