using System;
using Cysharp.Threading.Tasks;
using Gameplay.Manager;
using Unit.Trap;
using Unit.Trap.Activator;
using Unit.Trap.Fall;
using Unit.Trap.Hammer;
using Unit.Trap.Tower;
using UnityEngine;
using Zenject;

namespace Gameplay.Factory
{
    public class TrapFactory : Factory
    {
        [Inject] private PoolManager poolManager;
        
        public async UniTask<GameObject> Create(Type type)
        {
            GameObject result = type switch
            {
                _ when type == typeof(AxeController) => await poolManager.GetObjectAsync<AxeController>(),
                _ when type == typeof(HammerController) => await poolManager.GetObjectAsync<HammerController>(),
                _ when type == typeof(ButtonController) => await poolManager.GetObjectAsync<ButtonController>(),
                _ when type == typeof(PushController) => await poolManager.GetObjectAsync<PushController>(),
                _ when type == typeof(ThornController) => await poolManager.GetObjectAsync<ThornController>(),
                _ when type == typeof(DragonController) => await poolManager.GetObjectAsync<DragonController>(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return result;
        }
    }
}