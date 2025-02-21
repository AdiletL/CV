using System;
using System.Collections;
using Gameplay.UI.ScreenSpace;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit;
using Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Ability
{
    public abstract class Ability : IAbility
    {
        [Inject] protected DiContainer diContainer;
        [Inject] protected UICastTimer uiCastTimer;

        public event Action<int?, float, float> OnCountCooldown;
        public event Action<int?> OnActivated;
        public event Action<int?> OnStarted;
        public event Action<int?> OnFinished;
        public event Action<int?> OnExit;
        
        public int? InventorySlotID { get; protected set; }
        public GameObject GameObject { get; protected set; }
        public abstract AbilityType AbilityType { get; protected set; }
        public AbilityBehaviour AbilityBehaviour { get; protected set; }
        public InputType BlockedInputType { get; protected set; }
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
        public void SetBlockedInputType(InputType inputType) => this.BlockedInputType = inputType;
        public void SetAbilityBehaviour(AbilityBehaviour abilityBehaviour) => this.AbilityBehaviour = abilityBehaviour;
        public void SetCooldown(float cooldown) => this.Cooldown = cooldown;
        public void SetTimerCast(float timerCast) => this.TimerCast = timerCast;
        

        public virtual void Initialize()
        {
        }

        public virtual void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            if (IsCooldown)
            {
                Debug.Log($"{AbilityBehaviour} на перезарядке!");
                Exit();
                return;
            }

            if ((AbilityBehaviour & AbilityBehaviour.Passive) != 0)
            {
                Debug.Log($"{AbilityBehaviour} — пассивное умение, его нельзя активировать.");
                Exit();
                return;
            }

            if ((AbilityBehaviour & AbilityBehaviour.Hidden) != 0)
            {
                Debug.Log($"{AbilityBehaviour} скрыто и не может быть использовано.");
                Exit();
                return;
            }
            
            FinishedCallBack = finishedCallBack;
            isActivated = true;
            OnActivated?.Invoke(InventorySlotID);
        }

        protected void StartEffect()
        {
            if (isCasting) Exit();
            SetCursor(null);
            StartCooldown();
            StartCasting();
            OnStarted?.Invoke(InventorySlotID);
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
            if (Cooldown > 0)
            {
                IsCooldown = true;
                countCooldown = Cooldown;
            }
        }
        
        private void StartCasting()
        {
            if (TimerCast > 0)
            {
                isCasting = true;
                countTimerCast = TimerCast;
            }
        }
        
        protected virtual void AfterCast()
        {
        }
        
        protected void SetCursor(Texture2D texture2D) => Cursor.SetCursor(texture2D, Vector2.zero, CursorMode.Auto);
        
        public virtual void FinishEffect()
        {
            isActivated = false;
            FinishedCallBack?.Invoke();
            OnFinished?.Invoke(InventorySlotID);
            Exit();
        }
        public virtual void Exit()
        {
            OnExit?.Invoke(InventorySlotID);
        }
    }

    public abstract class AbilityConfig
    {
        public SO_BaseAbilityConfig SO_BaseAbilityConfig;
        public float Cooldown;
        public float TimerCast;
    }

    public abstract class AbilityBuilder<T> where T : IAbility
    {
        protected Ability ability;

        public AbilityBuilder(Ability instance)
        {
            ability = instance;
        }

        public AbilityBuilder<T> SetGameObject(GameObject gameObject)
        {
            ability.SetGameObject(gameObject);
            return this;
        }
        public AbilityBuilder<T> SetBlockedInputType(InputType inputType)
        {
            ability.SetBlockedInputType(inputType);
            return this;
        }
        public AbilityBuilder<T> SetAbilityBehaviour(AbilityBehaviour abilityBehaviour)
        {
            ability.SetAbilityBehaviour(abilityBehaviour);
            return this;
        }
        public AbilityBuilder<T> SetCooldown(float cooldown)
        {
            ability.SetCooldown(cooldown);
            return this;
        }
        public AbilityBuilder<T> SetTimerCast(float timerCast)
        {
            ability.SetTimerCast(timerCast);
            return this;
        }
        
        public IAbility Build()
        {
            return ability;
        }
    }
}