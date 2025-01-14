using System;
using ScriptableObjects.Unit;
using UnityEngine;

namespace Unit
{
    public abstract class UnitEndurance : MonoBehaviour, IEndurance
    {
        public event Action<EnduranceInfo> OnChangedEndurance;
        
        [SerializeField] protected UnitController unitController;
        [SerializeField] protected SO_UnitEndurance so_UnitEndurance;
        
        private EnduranceInfo enduranceInfo;
        
        public float MaxEndurance { get; protected set; }
        
        private float currentEndurance;
        public float CurrentEndurance
        {
            get => currentEndurance;
            set
            {
                if (value <= 0)
                {
                    currentEndurance = 0;
                    gameObject.SetActive(false);
                }
                else
                {
                    currentEndurance = value;
                }

                //Debug.Log(gameObject.name + " Current Endurance: " + currentEndurance);
                ExecuteEventChangedEndurance();
            }
        }
        
        
        protected virtual void ExecuteEventChangedEndurance()
        {
            enduranceInfo.CurrentEndurance = CurrentEndurance;
            enduranceInfo.MaxEndurance = MaxEndurance;
            OnChangedEndurance?.Invoke(enduranceInfo);
        }
        
        
        public virtual void Initialize()
        {
            enduranceInfo = new EnduranceInfo();
            MaxEndurance = so_UnitEndurance.MaxEndurance;
            CurrentEndurance = MaxEndurance;
        }

        public virtual void AddEndurance(float value)
        {
            CurrentEndurance += value;
        }

        public virtual void RemoveEndurance(float value)
        {
            CurrentEndurance -= value;
        }

        public virtual void IncreaseMaxEndurance(float value)
        {
            MaxEndurance += value;
        }
    }

    public class EnduranceInfo
    {
        public float MaxEndurance { get; set; }
        public float CurrentEndurance { get; set; }
    }
}