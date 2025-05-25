using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Gameplay.UI.ScreenSpace;
using ScriptableObjects.Ability;
using UnityEngine;
using Zenject;

namespace Gameplay.Ability
{
    public abstract class Ability : IAbility, 
        IStatsController<AbilityStatType>,
        IStatsController<UnitStatType>
    {
        [Inject] protected UICastTimer uiCastTimer;

        public event Action<int?, float, float> OnCountCooldown;
        public event Action<int?> OnStartedCast;
        public event Action<int?> OnFinishedCast;
        
        public int? InventorySlotID { get; protected set; }
        public GameObject GameObject { get; protected set; }
        public abstract AbilityType AbilityTypeID { get; protected set; }
        public AbilityBehaviour AbilityBehaviourID { get; protected set; }
        public Action FinishedCallBack { get; protected set; }
        public float TimerCast { get; protected set; }
        public bool IsCooldown { get; protected set; }
        
        protected SO_Ability so_Ability;
        private float countCooldown;
        private float countTimerCast;
        protected bool isActivated;
        protected bool isCasting;
        
        private Dictionary<AbilityStatType, Stat> abilityStats;
        private Dictionary<UnitStatType, Stat> unitStats;

        public Ability(SO_Ability so_Ability)
        {
            this.so_Ability = so_Ability;
        }
        
        public void SetInventorySlotID(int? slotID) => InventorySlotID = slotID;
        public void SetGameObject(GameObject gameObject) => this.GameObject = gameObject;

        public Stat GetStat(AbilityStatType statType)
        {
            return abilityStats.GetValueOrDefault(statType);
        }
        public Stat GetStat(UnitStatType unitStatType)
        {
            return unitStats.GetValueOrDefault(unitStatType);
        }

        public virtual void Initialize()
        {
            AbilityBehaviourID = so_Ability.AbilityBehaviour;
            TimerCast = so_Ability.TimerCast;

            abilityStats ??= new Dictionary<AbilityStatType, Stat>();
            foreach (var abilityStatConfig in so_Ability.AbilityStatConfigData.AbilityStatConfigs)
            {
                var stat = new Stat();
                foreach (var VARIABLE in abilityStatConfig.StatValuesConfig)
                    stat.AddValue(VARIABLE.GameValueConfig.Value, VARIABLE.StatValueTypeID);
                
                abilityStats[abilityStatConfig.AbilityStatTypeID] = stat;
            }
            
            unitStats ??= new Dictionary<UnitStatType, Stat>();
            foreach (var unitStatConfig in so_Ability.UnitStatConfigData.StatConfigs)
            {
                var stat = new Stat();
                foreach (var VARIABLE in unitStatConfig.StatValuesConfig)
                    stat.AddValue(VARIABLE.GameValueConfig.Value, VARIABLE.StatValueTypeID);
                
                unitStats[unitStatConfig.UnitStatTypeID] = stat;
            }
        }

        public virtual void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            if (IsCooldown)
            {
                Debug.Log($"{AbilityBehaviourID} на перезарядке!");
                Exit();
                return;
            }

            if ((AbilityBehaviourID & AbilityBehaviour.Passive) != 0)
            {
                Debug.Log($"{AbilityBehaviourID} — пассивное умение, его нельзя активировать.");
                Exit();
                return;
            }

            if ((AbilityBehaviourID & AbilityBehaviour.Hidden) != 0)
            {
                Debug.Log($"{AbilityBehaviourID} скрыто и не может быть использовано.");
                Exit();
                return;
            }
            
            FinishedCallBack = finishedCallBack;
            isActivated = true;
        }

        public virtual void StartEffect()
        {
            if (!isActivated) return;
            StartCooldown();
            StartCasting();
        }
        
        public virtual void Update()
        {
            if (IsCooldown)
            {
                countCooldown -= Time.deltaTime;
                if (countCooldown <= 0)
                    IsCooldown = false;
                OnCountCooldown?.Invoke(InventorySlotID, countCooldown, abilityStats[AbilityStatType.Cooldown].CurrentValue);
            }
            
            if (isCasting)
            {
                countTimerCast -= Time.deltaTime;
                uiCastTimer.UpdateTime(countTimerCast, TimerCast);
                if (countTimerCast <= 0)
                {
                    AfterCast();
                }
            }
        }

        private void StartCooldown()
        {
            IsCooldown = true;
            countCooldown = abilityStats[AbilityStatType.Cooldown].CurrentValue;
        }
        
        private void StartCasting()
        {
            isCasting = true;
            countTimerCast = TimerCast;
            if(TimerCast > .1f) uiCastTimer.Show();
            OnStartedCast?.Invoke(InventorySlotID);
        }
        
        protected virtual void AfterCast()
        {
            isCasting = false;
            uiCastTimer.Hide();
            FinishedCallBack?.Invoke();
            OnFinishedCast?.Invoke(InventorySlotID);
        }
        
        public virtual void Exit()
        {
            isActivated = false;
            isCasting = false;
            uiCastTimer.Hide();
        }
    }
    
    [System.Serializable]
    public class AbilityStatConfig : StatConfig
    {
        public AbilityStatType AbilityStatTypeID;
    }

    [System.Serializable]
    public class AbilityStatConfigData
    {
        public AbilityStatConfig[] AbilityStatConfigs;
    }
}