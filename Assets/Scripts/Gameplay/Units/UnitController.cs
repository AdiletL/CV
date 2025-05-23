﻿using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay.Unit
{
    [SelectionBase]
    [RequireComponent(typeof(UnitCenter))]
    public abstract class UnitController : MonoBehaviour, IUnit, IActivatable
    {
        [Inject] protected DiContainer diContainer;
        
        [FormerlySerializedAs("componentsInGameObjects")] 
        [SerializeField] protected ComponentsInGameObjects components;
        
        [field: SerializeField, Space(10)] public GameObject VisualParent { get; protected set; }

        protected UnitCenter unitCenter;
        protected UnitRenderer unitRenderer;

        public bool IsActive { get; protected set; }
        
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

        public virtual void Activate()
        {
            var iActivatables = components.GetComponentsInGameObjects<IActivatable>();
            for (int i = 0; i < iActivatables.Count; i++)
                if(!ReferenceEquals(iActivatables[i], this)) 
                    iActivatables[i].Activate();
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            var iActivatables = components.GetComponentsInGameObjects<IActivatable>();
            for (int i = 0; i < iActivatables.Count; i++)
                if(!ReferenceEquals(iActivatables[i], this)) 
                    iActivatables[i].Deactivate();
            IsActive = false;
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