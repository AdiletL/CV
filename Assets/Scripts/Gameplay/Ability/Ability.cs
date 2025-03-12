using System;
using System.Collections;
using Gameplay.UI.ScreenSpace;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Ability
{
    public abstract class Ability : IAbility
    {
        [Inject] protected UICastTimer uiCastTimer;

        public event Action<int?, float, float> OnCountCooldown;
        public event Action<int?> OnStartedCast;
        public event Action<int?> OnFinishedCast;
        
        public int? InventorySlotID { get; protected set; }
        public GameObject GameObject { get; protected set; }
        public abstract AbilityType AbilityTypeID { get; protected set; }
        public AbilityBehaviour AbilityBehaviourID { get; protected set; }
        public InputType BlockedInputTypeID { get; protected set; }
        public Action FinishedCallBack { get; protected set; }
        public float Cooldown { get; protected set; }
        public float TimerCast { get; protected set; }
        public bool IsCooldown { get; protected set; }

        private float countCooldown;
        private float countTimerCast;
        protected bool isActivated;
        protected bool isCasting;
        

        public void SetInventorySlotID(int? slotID) => InventorySlotID = slotID;
        public void SetGameObject(GameObject gameObject) => this.GameObject = gameObject;
        public void SetBlockedInputType(InputType inputType) => this.BlockedInputTypeID = inputType;
        public void SetAbilityBehaviour(AbilityBehaviour abilityBehaviour) => this.AbilityBehaviourID = abilityBehaviour;
        public void SetCooldown(float cooldown) => this.Cooldown = cooldown;
        public void SetTimerCast(float timerCast) => this.TimerCast = timerCast;
        

        public virtual void Initialize()
        {
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

        protected void StartEffect()
        {
            if (isCasting) Exit();
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

    public abstract class AbilityConfig
    {
        public SO_BaseAbilityConfig SO_BaseAbilityConfig;
        public float Cooldown;
        public float TimerCast;
    }

    public abstract class AbilityBuilder
    {
        protected Ability ability;

        public AbilityBuilder(Ability instance)
        {
            ability = instance;
        }

        public AbilityBuilder SetGameObject(GameObject gameObject)
        {
            ability.SetGameObject(gameObject);
            return this;
        }
        public AbilityBuilder SetBlockedInputType(InputType inputType)
        {
            ability.SetBlockedInputType(inputType);
            return this;
        }
        public AbilityBuilder SetAbilityBehaviour(AbilityBehaviour abilityBehaviour)
        {
            ability.SetAbilityBehaviour(abilityBehaviour);
            return this;
        }
        public AbilityBuilder SetCooldown(float cooldown)
        {
            ability.SetCooldown(cooldown);
            return this;
        }
        public AbilityBuilder SetTimerCast(float timerCast)
        {
            ability.SetTimerCast(timerCast);
            return this;
        }
        
        public Ability Build()
        {
            return ability;
        }
    }
}