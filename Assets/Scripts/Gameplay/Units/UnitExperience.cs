using System;
using UnityEngine;
using Gameplay.Experience;
using ScriptableObjects.Unit;
using Zenject;

namespace Unit
{
    public abstract class UnitExperience : MonoBehaviour, IExperience, ILevel
    {
        [Inject] protected DiContainer diContainer;
        
        public static event Action<AoeExperienceInfo> OnGiveAoeExperience;
        
        [SerializeField] protected UnitController unitController;
        [SerializeField] protected SO_UnitExperience so_UnitExperience;
        
        private AoeExperienceInfo aoeExperienceInfo;
        
        public ICountExperience ICountExperienceCalculate { get; protected set; }
        public Stat ExperienceStat { get; } = new Stat();
        public Stat LevelStat { get; } = new Stat();

        public float RangeTakeExperience { get; protected set; }
        public bool IsTakeLevel { get; protected set; }
        public bool IsTakeExperience { get; protected set; }
        public bool IsGiveExperience { get; protected set; }
        

        public virtual void Initialize()
        {
            LevelStat.AddValue(so_UnitExperience.StartLevel);
            ExperienceStat.AddValue(so_UnitExperience.Experience);
            RangeTakeExperience = so_UnitExperience.RangeTakeExperience;
            IsTakeLevel = so_UnitExperience.IsTakeLevel;
            IsTakeExperience = so_UnitExperience.IsTakeExperience;
            IsGiveExperience = so_UnitExperience.IsGiveExperience;
            
            aoeExperienceInfo = new AoeExperienceInfo((int)ExperienceStat.CurrentValue, RangeTakeExperience, gameObject);
            ICountExperienceCalculate = new ExponentialICountExperience();
            diContainer.Inject(ICountExperienceCalculate);
        }
        
        public virtual void AddExperience(int experience)
        {
            if(!IsTakeExperience) return;
            ExperienceStat.AddValue(experience);
            CheckLevelUp();
            Debug.Log(gameObject.name + " Added Experience " + ExperienceStat.CurrentValue);
        }

        protected virtual void CheckLevelUp()
        {
            int experienceToNextLevel = ICountExperienceCalculate.CalculateExperienceForNextLevel((int)LevelStat.CurrentValue, (int)ExperienceStat.CurrentValue);
        
            while (ExperienceStat.CurrentValue >= experienceToNextLevel)
            {
                LevelUp(1);
                experienceToNextLevel = ICountExperienceCalculate.CalculateExperienceForNextLevel((int)LevelStat.CurrentValue, (int)ExperienceStat.CurrentValue);
            }
        }
        public virtual void LevelUp(int amount)
        {
            if(!IsTakeLevel) return;
            LevelStat.AddValue(amount);
            Debug.Log(gameObject.name + " Level Up! New Level: " + LevelStat.CurrentValue);
        }
        

        public virtual void GiveExperience()
        {
            if(!IsGiveExperience) return;
            OnGiveAoeExperience?.Invoke(aoeExperienceInfo);
        }
    }

    public class AoeExperienceInfo
    {
        public int TotalExperience { get; private set; }
        public float RangeTakeExperience { get; private set; }
        public GameObject Owner { get; private set; }

        public AoeExperienceInfo(int totalExperience, float rangeTakeExperience, GameObject owner)
        {
            TotalExperience = totalExperience;
            RangeTakeExperience = rangeTakeExperience;
            Owner = owner;
        }
    }
}