using System;
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
    public class TrapFactoryMethod : FactoryMethod
    {
        private PoolManager poolManager;

        [Inject]
        private void Construct(PoolManager poolManager)
        {
            this.poolManager = poolManager;
        }
        
        public override T Create<T>(Type type)
        {
            throw new ArgumentException();
        }

        public GameObject Create(Type type)
        {
            GameObject result = type switch
            {
                _ when type == typeof(AxeController) => poolManager.GetObject<AxeController>(),
                _ when type == typeof(FallController) => poolManager.GetObject<FallController>(),
                _ when type == typeof(HammerController) => poolManager.GetObject<HammerController>(),
                _ when type == typeof(ButtonController) => poolManager.GetObject<ButtonController>(),
                _ when type == typeof(PushController) => poolManager.GetObject<PushController>(),
                _ when type == typeof(ThornController) => poolManager.GetObject<ThornController>(),
                _ when type == typeof(DragonController) => poolManager.GetObject<DragonController>(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return result;
        }
    }
}