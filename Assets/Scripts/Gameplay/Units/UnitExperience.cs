using System;
using UnityEngine;
using Gameplay.Experience;
using ScriptableObjects.Unit;
using Unit.Character.Player;
using UnityEngine.Serialization;
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
        
        public IExperience ExperienceCalculate { get; protected set; }
        
        public float RangeTakeExperience { get; protected set; }
        public int CurrentLevel { get; protected set; }
        public int CurrentExperience { get; protected set; }

        public bool IsTakeLevel { get; protected set; }
        public bool IsTakeExperience { get; protected set; }
        public bool IsGiveExperience { get; protected set; }
        

        public virtual void Initialize()
        {
            CurrentLevel = so_UnitExperience.StartLevel;
            CurrentExperience = 0;
            CurrentExperience = so_UnitExperience.Experience;
            RangeTakeExperience = so_UnitExperience.RangeTakeExperience;
            IsTakeLevel = so_UnitExperience.IsTakeLevel;
            IsTakeExperience = so_UnitExperience.IsTakeExperience;
            IsGiveExperience = so_UnitExperience.IsGiveExperience;
            
            aoeExperienceInfo = new AoeExperienceInfo(
                CurrentExperience, RangeTakeExperience, gameObject);
            ExperienceCalculate = new ExponentialExperience();
            diContainer.Inject(ExperienceCalculate);
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
            int experienceToNextLevel = ExperienceCalculate.CalculateExperienceForNextLevel(CurrentLevel, CurrentExperience);
        
            while (CurrentExperience >= experienceToNextLevel)
            {
                LevelUp(1);
                experienceToNextLevel = ExperienceCalculate.CalculateExperienceForNextLevel(CurrentLevel, CurrentExperience);
            }
        }
        public virtual void LevelUp(int amount)
        {
            if(!IsTakeLevel) return;
            
            CurrentLevel += amount;
            Debug.Log(gameObject.name + " Level Up! New Level: " + CurrentLevel);
        }

        public virtual void IncreaseLevel(int value)
        {
            LevelUp(value);
        }

        public virtual void OnDeath()
        {
            if(!IsGiveExperience) return;
            Debug.Log(diContainer.TryResolve<ExperienceSystem>());
            OnGiveAoeExperience?.Invoke(aoeExperienceInfo);
        }
    }

    public class AoeExperienceInfo
    {
        public int TotalExperience { get; }
        public float RangeTakeExperience { get; }
        public GameObject GameObject { get; }

        public AoeExperienceInfo(int totalExperience, float rangeTakeExperience, GameObject gameObject)
        {
            TotalExperience = totalExperience;
            RangeTakeExperience = rangeTakeExperience;
            GameObject = gameObject;
        }
    }
}