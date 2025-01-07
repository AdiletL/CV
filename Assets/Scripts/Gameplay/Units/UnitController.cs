using System;
using UnityEngine;

namespace Unit
{
    [SelectionBase]
    [RequireComponent(typeof(UnitCenter))]
    public abstract class UnitController : MonoBehaviour, IUnit
    {
        [SerializeField] protected ComponentsInGameObjects components;
        
        [Space(10)]
        [SerializeField] protected GameObject visualParent;

        public abstract UnitType UnitType { get; }
        

        public T GetComponentInUnit<T>()
        {
            return components.GetComponentFromArray<T>();
        }

        public virtual void Initialize()
        {
            components.Initialize();
        }

        public void Show() => visualParent?.SetActive(true);
        public void Hide() => visualParent?.SetActive(false);

        public abstract void Appear();
    }
}