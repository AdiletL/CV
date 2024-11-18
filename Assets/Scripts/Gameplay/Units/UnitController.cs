using System;
using UnityEngine;

namespace Unit
{
    [RequireComponent(typeof(UnitCenter))]
    public abstract class UnitController : MonoBehaviour, IUnit
    {
        [field: SerializeField] public ComponentsInGameObjects components { get; protected set; }

        public abstract UnitType UnitType { get; }

        public virtual void Initialize()
        {
            components.Initialize();
        }
    }
}