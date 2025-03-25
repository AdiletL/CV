using System;
using System.Collections.Generic;
using Calculate;
using Gameplay.Effect;
using Gameplay.UI.ScreenSpace;
using ScriptableObjects.Unit.Item;
using UnityEngine;
using Zenject;
using ValueType = Calculate.ValueType;

namespace Gameplay.Unit.Item
{
    public abstract class Item : IItem
    {
        [Inject] private UICastTimer uiCastTimer;
        
        public event Action<int?> OnStartedCast;
        public event Action<int?> OnFinishedCast;
        public event Action<int?, float, float> OnCountCooldown;
        
        public int? InventorySlotID { get; protected set; }
        public GameObject Owner { get; protected set; }
        public abstract string ItemName { get; protected set; }
        public ItemCategory ItemCategoryID { get; protected set; }
        public ItemBehaviour ItemBehaviourID { get; protected set; }
        public abstract ItemUsageType ItemUsageTypeID { get; }
        public Action FinishedCallBack { get; protected set; }
        public int Amount { get; protected set; }
        public float Cooldown { get; protected set; }
        public float TimerCast { get; protected set; }
        public bool IsCooldown { get; protected set; }
        public StatConfig[] Stats { get; protected set; }
        public List<Ability.Ability> Abilities { get; protected set; }

        protected SO_Item so_Item;
        private float countCooldown;
        private float countTimerCast;
        protected bool isActivated;
        protected bool isCasting;

        private List<float> addedStatValues;

        public Item(SO_Item so_Item)
        {
            this.so_Item = so_Item;
        }
        
        public void SetInventorySlotID(int? slotID) => InventorySlotID = slotID;
        public void SetOwner(GameObject gameObject) => this.Owner = gameObject;
        public void SetAmountItem(int amount) => Amount = amount;
        public void SetStats(StatConfig[] stats)
        {
            Stats = new StatConfig[stats.Length];
            for (int i = 0; i < stats.Length; i++)
            {
                Stats[i] = stats[i];
            }
        }

        public virtual void Initialize()
        {
            ItemCategoryID = so_Item.ItemCategoryID;
            ItemBehaviourID = so_Item.ItemBehaviourID;
            Cooldown = so_Item.Cooldown;
            TimerCast = so_Item.TimerCast;
        }

        public virtual void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            if (IsCooldown)
            {
                Debug.Log($"{ItemBehaviourID} на перезарядке!");
                Exit();
                return;
            }
            
            if ((ItemBehaviourID & ItemBehaviour.Passive) != 0)
            {
                Debug.Log($"{ItemBehaviourID} — пассивное умение, его нельзя активировать.");
                Exit();
                return;
            }
            
            FinishedCallBack = finishedCallBack;
            isActivated = true;
        }
        
        public virtual void StartEffect()
        {
            if(!isActivated) return;
            StartCasting();
            StartCooldown();
        }
        
        public virtual void Update()
        {
            if (IsCooldown)
            {
                countCooldown -= Time.deltaTime;
                if (countCooldown <= 0)
                    IsCooldown = false;
                OnCountCooldown?.Invoke(InventorySlotID, countCooldown, Cooldown);
            }
            
            if (isCasting)
            {
                countTimerCast -= Time.deltaTime;
                uiCastTimer.UpdateTime(countTimerCast, TimerCast);
                if (countTimerCast <= 0)
                {
                    AfterCast();
                    isCasting = false;
                }
            }
        }

        private void StartCooldown()
        {
            IsCooldown = true;
            countCooldown = Cooldown;
        }

        private void StartCasting()
        {
            isCasting = true;
            countTimerCast = TimerCast;
            uiCastTimer.Show();
            OnStartedCast?.Invoke(InventorySlotID);
        }
        
        protected virtual void AfterCast()
        {
            isCasting = false;
            uiCastTimer.Hide();
            FinishedCallBack?.Invoke();
            OnFinishedCast?.Invoke(InventorySlotID);
        }

        public virtual void AddStatsToUnit()
        {
            addedStatValues ??= new List<float>();
            addedStatValues.Clear();
            
            var unitStatController = Owner.GetComponent<UnitStatsController>();
            Stat stat = null;
            float result = 0;
            foreach (var config in Stats)
            {
                stat = unitStatController.GetStat(config.StatTypeID);
                foreach (var statValue in config.StatValuesConfig)
                {
                    var gameValue = new GameValue(statValue.Value, statValue.ValueTypeID);
                    result = gameValue.Calculate(stat.GetValue(statValue.StatValueTypeID));
                    stat.AddValue(result, statValue.StatValueTypeID); 
                    addedStatValues.Add(result);
                }
            }

            AddEffectToUnit();
        }

        public virtual void RemoveStatsFromUnit()
        {
            var unitStatController = Owner.GetComponent<UnitStatsController>();
            Stat stat = null;
            int index = 0;
            foreach (var config in Stats)
            {
                stat = unitStatController.GetStat(config.StatTypeID);
                foreach (var statValue in config.StatValuesConfig)
                {
                    stat.RemoveValue(addedStatValues[index], statValue.StatValueTypeID);
                    index++;
                }
            }

            RemoveEffectFromUnit();
        }
        
        public virtual void Exit()
        {
            isActivated = false;
            isCasting = false;
            uiCastTimer.Hide();
        }

        protected void AddAbility(Ability.Ability ability)
        {
            Abilities ??= new ();
            Abilities.Add(ability);
        }

        protected void RemoveAbility(Ability.Ability ability) => Abilities.Remove(ability);
        
        protected virtual void AddEffectToUnit()
        {
            
        }
        protected virtual void RemoveEffectFromUnit()
        {
            
        }
        
        public void AddAmount(int amount) => Amount += amount;
        public void RemoveAmount(int amount) => Amount -= amount;

        public virtual void ShowContextMenu()
        {
            
        }
        public virtual void HideContextMenu()
        {
            
        }
    }
}