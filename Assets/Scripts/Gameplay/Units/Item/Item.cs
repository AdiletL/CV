using System;
using System.Collections.Generic;
using Calculate;
using Gameplay.UI.ScreenSpace;
using Unit;
using UnityEngine;
using Zenject;
using ValueType = Calculate.ValueType;

namespace Gameplay.Units.Item
{
    public abstract class Item : IItem
    {
        [Inject] private UICastTimer uiCastTimer;
        
        public event Action<int?> OnStartedCast;
        public event Action<int?> OnFinishedCast;
        public event Action<int?, float, float> OnCountCooldown;
        
        public int? InventorySlotID { get; protected set; }
        public GameObject OwnerGameObject { get; protected set; }
        public abstract ItemName ItemNameID { get; protected set; }
        public ItemCategory ItemCategoryID { get; protected set; }
        public ItemBehaviour ItemBehaviourID { get; protected set; }
        public InputType BlockInputTypeID { get; protected set; }
        public Action FinishedCallBack { get; protected set; }
        public int Amount { get; protected set; }
        public float Cooldown { get; protected set; }
        public float TimerCast { get; protected set; }
        public bool IsCooldown { get; protected set; }
        public StatConfig[] Stats { get; protected set; }
        public List<Ability.Ability> Abilities { get; protected set; }

        private float countCooldown;
        private float countTimerCast;
        protected bool isActivated;
        protected bool isCasting;

        private List<float> additionalPercentStatsToUnit;
        
        public void SetInventorySlotID(int? slotID) => InventorySlotID = slotID;
        public void SetOwnerGameObject(GameObject gameObject) => this.OwnerGameObject = gameObject;
        public void SetItemCategory(ItemCategory itemCategory) => ItemCategoryID = itemCategory;
        public void SetItemBehaviour(ItemBehaviour itemBehaviour) => ItemBehaviourID = itemBehaviour;
        public void SetCooldown(float cooldown) => this.Cooldown = cooldown;
        public void SetTimerCast(float timerCast) => this.TimerCast = timerCast;
        public void SetAmountItem(int amount) => Amount = amount;
        public void SetBlockInputType(InputType inputType) => BlockInputTypeID = inputType;

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
        
        public void StartEffect()
        {
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

        public virtual void LateUpdate()
        {
            
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


        public virtual void AddStatsFromUnit()
        {
            additionalPercentStatsToUnit ??= new List<float>();
            additionalPercentStatsToUnit.Clear();
            
            var unitStatController = OwnerGameObject.GetComponent<UnitStatsController>();
            foreach (var config in Stats)
            {
                foreach (var statValues in config.StatValuesConfig)
                {
                    var unitStats = unitStatController.GetStat(config.StatTypeID);
                    foreach (var unitStat in unitStats)
                    {
                        var gameValue = new GameValue(statValues.Value, statValues.ValueTypeID);
                        var result = gameValue.Calculate(unitStat.CurrentValue);
                        if (statValues.ValueTypeID == ValueType.Percent)
                            additionalPercentStatsToUnit.Add(result);
                        
                        switch (statValues.StatValueTypeID)
                        {
                            case StatValueType.Nothing: break;
                            case StatValueType.Current: unitStat.AddValue(result); break;
                            case StatValueType.Minimum: unitStat.AddMinValue(result); break;
                            case StatValueType.Maximum: unitStat.AddMaxValue(result); break;
                            default: throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        public virtual void RemoveStatsFromUnit()
        {
            var unitStatController = OwnerGameObject.GetComponent<UnitStatsController>();
            
            foreach (var config in Stats)
            {
                foreach (var VARIABLE in config.StatValuesConfig)
                {
                    var unitStats = unitStatController.GetStat(config.StatTypeID);
                    for (int i = 0; i < unitStats.Count; i++)
                    {
                        switch (VARIABLE.StatValueTypeID)
                        {
                            case StatValueType.Nothing: break;

                            case StatValueType.Current:
                                if (VARIABLE.ValueTypeID == ValueType.Percent)
                                    unitStats[i].RemoveValue(additionalPercentStatsToUnit[i]);
                                else
                                    unitStats[i].RemoveValue(VARIABLE.Value);
                                break;

                            case StatValueType.Minimum:
                                if (VARIABLE.ValueTypeID == ValueType.Percent)
                                    unitStats[i].RemoveMinValue(additionalPercentStatsToUnit[i]);
                                else
                                    unitStats[i].RemoveMinValue(VARIABLE.Value);
                                break;

                            case StatValueType.Maximum:
                                if (VARIABLE.ValueTypeID == ValueType.Percent)
                                    unitStats[i].RemoveMaxValue(additionalPercentStatsToUnit[i]);
                                else
                                    unitStats[i].RemoveMaxValue(VARIABLE.Value);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        public virtual void PutOn()
        {
            
        }

        public virtual void TakeOff()
        {
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

        protected void RemoveAbility(Ability.Ability ability)
        {
            Abilities.Remove(ability);
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
    
    public abstract class ItemBuilder<T> where T : IItem
    {
        protected Item item;

        public ItemBuilder(Item item)
        {
            this.item = item;
        }

        public ItemBuilder<T> SetGameObject(GameObject gameObject)
        {
            item.SetOwnerGameObject(gameObject);
            return this;
        }
        
        public ItemBuilder<T> SetItemCategory(ItemCategory itemCategoryID)
        {
            item.SetItemCategory(itemCategoryID);
            return this;
        }
        
        public ItemBuilder<T> SetItemBehaviour(ItemBehaviour itemBehaviourID)
        {
            item.SetItemBehaviour(itemBehaviourID);
            return this;
        }
        
        public ItemBuilder<T> SetBlockInput(InputType inputType)
        {
            item.SetBlockInputType(inputType);
            return this;
        }
        
        public ItemBuilder<T> SetCooldown(float cooldown)
        {
            item.SetCooldown(cooldown);
            return this;
        }
        
        public ItemBuilder<T> SetTimerCast(float timerCast)
        {
            item.SetTimerCast(timerCast);
            return this;
        }

        public Item Build()
        {
            return item;
        }
    }
}