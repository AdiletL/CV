using System;
using UnityEngine;

namespace Gameplay.Factory
{
    public abstract class FactoryMethod : IFactory
    {
        public abstract T Create<T>(Type type);
    }
}