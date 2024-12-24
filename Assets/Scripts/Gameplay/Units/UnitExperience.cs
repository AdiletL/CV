using System;
using UnityEngine;
using Gameplay.Experience;
using ScriptableObjects.Unit;
using Zenject;

namespace Unit
{
    public abstract class UnitExperience : MonoBehaviour, IUnitExperience, IUnitLevel
    {
        [Inject] protected DiContainer diContainer;
        
        public static event Action<AoeExperienceInfo> OnGiveAoeExperience;
        
        [SerializeField] protected UnitController unitController;
        [SerializeField] protected SO_UnitExperience so_UnitExperience;
        
        private AoeExperienceInfo aoeExperienceInfo;
        
        private int giveExperience;
        
        public IExperience ExperienceCalculate { get; protected set; }
        
        public int CurrentLevel { get; protected set; }
        public int CurrentExperience { get; protected set; }

        public int GiveExperience
        {
            get => giveExperience + CurrentExperience;
            set => giveExperience = value;
        }
        public int RangeTakeExperience { get; protected set; }
        
        public bool IsTakeLevel { get; protected set; }
        public bool IsTakeExperience { get; protected set; }
        public bool IsGiveExperience { get; protected set; }

        public bool IsRangeTakeExperience(GameObject target)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= RangeTakeExperience)
                return true;

            return false;
        }


        public virtual void Initialize()
        {
            aoeExperienceInfo = new AoeExperienceInfo();
            ExperienceCalculate = new ExponentialExperience();
            diContainer.Inject(ExperienceCalculate);

            CurrentLevel = so_UnitExperience.StartLevel;
            CurrentExperience = 0;
            GiveExperience = so_UnitExperience.GiveExperience;
            RangeTakeExperience = so_UnitExperience.RangeTakeExperience;
            IsTakeLevel = so_UnitExperience.IsTakeLevel;
            IsTakeExperience = so_UnitExperience.IsTakeExperience;
            IsGiveExperience = so_UnitExperience.IsGiveExperience;
            unitController.GetComponentInUnit<UnitHealth>().OnDeath += OnDeath;
        }
        
        public virtual void AddExperience(int experience)
        {
            if(!IsTakeExperience) return;
            
            CurrentExperience += experience;
            CheckLevelUp();
            Debug.Log(gameObject.name + " Added Experience " + CurrentExperience);
        }

        protected virtual void CheckLevelUp()
        {
            int experienceToNextLevel = ExperienceCalculate.CalculateExperienceForNextLevel(CurrentLevel);
        
            while (CurrentExperience >= experienceToNextLevel)
            {
                LevelUp(1);
                experienceToNextLevel = ExperienceCalculate.CalculateExperienceForNextLevel(CurrentLevel);
            }
        }
        public virtual void LevelUp(int amount)
        {
            if(!IsTakeLevel) return;
            
            CurrentLevel += amount;
            Debug.Log(gameObject.name + " Level Up! New Level: " + CurrentLevel);
        }

        public virtual void IncreaseLevel(Unit.IState states)
        {
            if (states is UnitLevelStates unit)
            {
                AddExperience(unit.Experience);
                LevelUp(unit.Level);
            }
        }

        protected virtual void OnDeath()
        {
            if(!IsGiveExperience) return;

            var damaging = unitController.GetComponentInUnit<UnitHealth>().Damaging;
            if (damaging.TryGetComponent(out IUnitExperience unitExperience))
                unitExperience.AddExperience(GiveExperience);
            
            aoeExperienceInfo.TotalExperience = GiveExperience;
            aoeExperienceInfo.GameObject = gameObject;
            aoeExperienceInfo.Damaging = damaging;
            OnGiveAoeExperience?.Invoke(aoeExperienceInfo);
        }

        private void OnDestroy()
        {
            unitController.GetComponentInUnit<UnitHealth>().OnDeath -= OnDeath;
        }
    }

    public class AoeExperienceInfo
    {
        public int TotalExperience { get; set; }
        public GameObject GameObject { get; set; }
        public GameObject Damaging { get; set; }
    }
}