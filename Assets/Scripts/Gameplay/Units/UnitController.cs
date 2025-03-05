using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Unit
{
    [SelectionBase]
    [RequireComponent(typeof(UnitCenter))]
    public abstract class UnitController : MonoBehaviour, IUnit
    {
        [Inject] protected DiContainer diContainer;
        
        [FormerlySerializedAs("componentsInGameObjects")] 
        [SerializeField] protected ComponentsInGameObjects components;
        
        [field: SerializeField, Space(10)] public GameObject VisualParent { get; protected set; }

        protected UnitCenter unitCenter;
        protected UnitRenderer unitRenderer;
        
        public T GetComponentInUnit<T>() where T: class
        {
            return components.GetComponentFromArray<T>();
        }

        public bool TryGetComponentInUnit<T>(out T component) where T: class
        {
            return components.TryGetComponentFromArray(out component);
        }

        public virtual void Initialize()
        {
            components.Initialize();
            unitCenter = GetComponentInUnit<UnitCenter>();
            if (TryGetComponentInUnit(out unitRenderer))
            {
                diContainer.Inject(unitRenderer);
                unitRenderer.Initialize();
            }
        }

        public abstract void Appear();
        public abstract void Disappear();
        
        public void Show() => VisualParent?.SetActive(true);
        public void Hide() => VisualParent?.SetActive(false);


        public virtual void MoveDirection(Vector3 direction, float speed)
        {
            transform.Translate(direction * (speed * Time.deltaTime));
        }

        public virtual void ChangePosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}