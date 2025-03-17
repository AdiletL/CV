using System;
using UnityEngine;
using Gameplay.Experience;
using ScriptableObjects.Unit;
using Zenject;

namespace Gameplay.Unit
{
    public abstract class UnitExperience : MonoBehaviour, IExperience, ILevel
    {
        [Inject] protected DiContainer diContainer;
        
        public static event Action<AoeExperienceInfo> OnGiveAoeExperience;
        
        [SerializeField] protected UnitController unitController;
        [SerializeField] protected SO_UnitExperience so_UnitExperience;
        
        private AoeExperienceInfo aoeExperienceInfo;
        
        public ICountExperience ICountExperience { get; protected set; }
        public Stat ExperienceStat { get; } = new Stat();
        public Stat LevelStat { get; } = new Stat();

        public float RangeTakeExperience { get; protected set; }
        public bool IsTakeLevel { get; protected set; }
        public bool IsTakeExperience { get; protected set; }
        public bool IsGiveExperience { get; protected set; }
        

        public virtual void Initialize()
        {
            LevelStat.AddCurrentValue(so_UnitExperience.StartLevel);
            ExperienceStat.AddCurrentValue(so_UnitExperience.CurrentExperience);
            ExperienceStat.AddMaxValue(so_UnitExperience.MaxExperience);
            RangeTakeExperience = so_UnitExperience.RangeTakeExperience;
            IsTakeLevel = so_UnitExperience.IsTakeLevel;
            IsTakeExperience = so_UnitExperience.IsTakeExperience;
            IsGiveExperience = so_UnitExperience.IsGiveExperience;
            
            aoeExperienceInfo = new AoeExperienceInfo((int)ExperienceStat.CurrentValue, RangeTakeExperience, gameObject);
            ICountExperience = new ExponentialICountExperience();
            diContainer.Inject(ICountExperience);
        }
        
        public virtual void AddExperience(int experience)
        {
            if(!IsTakeExperience) return;
            ExperienceStat.AddCurrentValue(experience);
            CheckLevelUp();
            Debug.Log(gameObject.name + " Added Experience " + ExperienceStat.CurrentValue);
        }

        protected virtual void CheckLevelUp()
        {
            while (ExperienceStat.CurrentValue >= ExperienceStat.MaximumValue)
            {
                LevelUp(1);
                UpdateMaxExperience();
            }
        }
        public virtual void LevelUp(int amount)
        {
            if(!IsTakeLevel) return;
            LevelStat.AddCurrentValue(amount);
            UpdateMaxExperience();
            Debug.Log(gameObject.name + " Level Up! New Level: " + LevelStat.CurrentValue);
        }

        private void UpdateMaxExperience()
        {
            int experienceToNextLevel = ICountExperience.CalculateExperienceForNextLevel((int)LevelStat.CurrentValue, (int)ExperienceStat.CurrentValue);
            ExperienceStat.AddMaxValue(experienceToNextLevel);
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