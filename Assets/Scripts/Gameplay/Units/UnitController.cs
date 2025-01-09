using System;
using UnityEngine;

namespace Unit
{
    [SelectionBase]
    [RequireComponent(typeof(UnitCenter))]
    public abstract class UnitController : MonoBehaviour, IUnit
    {
        [SerializeField] protected ComponentsInGameObjects components;
        
        [field: SerializeField, Space(10)] public GameObject VisualParent { get; protected set;}

        public abstract UnitType UnitType { get; }
        

        public T GetComponentInUnit<T>()
        {
            return components.GetComponentFromArray<T>();
        }

        public virtual void Initialize()
        {
            components.Initialize();
        }

        public void Show() => VisualParent?.SetActive(true);
        public void Hide() => VisualParent?.SetActive(false);

        public abstract void Appear();

        public virtual void MoveDirection(Vector3 direction, float speed)
        {
            transform.Translate(direction * (speed * Time.deltaTime));
        }
    }
}