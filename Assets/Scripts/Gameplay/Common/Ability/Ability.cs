using System;
using System.Collections;
using Gameplay.AttackModifier;
using Gameplay.Effect;
using Gameplay.UI.ScreenSpace;
using ScriptableObjects.Ability;
using UnityEngine;
using Zenject;

namespace Gameplay.Ability
{
    public abstract class Ability : IAbility
    {
        [Inject] protected UICastTimer uiCastTimer;
        [Inject] protected SO_AbilityContainer SoAbilityContainer;

        public event Action<int?, float, float> OnCountCooldown;
        public event Action<int?> OnStartedCast;
        public event Action<int?> OnFinishedCast;
        
        public int? InventorySlotID { get; protected set; }
        public GameObject GameObject { get; protected set; }
        public abstract AbilityType AbilityTypeID { get; protected set; }
        public AbilityBehaviour AbilityBehaviourID { get; protected set; }
        public Action FinishedCallBack { get; protected set; }
        public float Range { get; protected set; }
        public float Cooldown { get; protected set; }
        public float TimerCast { get; protected set; }
        public bool IsCooldown { get; protected set; }

        protected SO_Ability so_Ability;
        private float countCooldown;
        private float countTimerCast;
        protected bool isActivated;
        protected bool isCasting;

        public Ability(SO_Ability so_Ability)
        {
            this.so_Ability = so_Ability;
        }
        
        public void SetInventorySlotID(int? slotID) => InventorySlotID = slotID;
        public void SetGameObject(GameObject gameObject) => this.GameObject = gameObject;
        

        public virtual void Initialize()
        {
            AbilityBehaviourID = SoAbilityContainer.GetAbilityConfig(so_Ability.AbilityTypeID).AbilityBehaviour;
            Cooldown = so_Ability.Cooldown;
            TimerCast = so_Ability.TimerCast;
            Range = so_Ability.Range;
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

        private void StartCooldown()
        {
            IsCooldown = true;
            countCooldown = Cooldown;
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
}