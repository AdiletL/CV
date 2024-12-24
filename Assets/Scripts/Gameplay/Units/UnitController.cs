using System;
using UnityEngine;

namespace Unit
{
    [RequireComponent(typeof(UnitCenter))]
    public abstract class UnitController : MonoBehaviour, IUnit
    {
        [SerializeField] protected ComponentsInGameObjects components;

        public abstract UnitType UnitType { get; }
        
        public abstract T GetComponentInUnit<T>() where T : class;
        
        public virtual void Initialize()
        {
            components.Initialize();
        }
    }
}