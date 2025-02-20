using System;
using System.Collections;
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
        public AnimationClip CastClip { get; protected set; }
        public float Cooldown { get; protected set; }
        public bool IsCooldown { get; protected set; }

        private float countCooldown;
        protected bool isActivated;
        

        public void SetInventorySlotID(int? slotID) => InventorySlotID = slotID;
        public void SetGameObject(GameObject gameObject) => this.GameObject = gameObject;
        public void SetBlockedInputType(InputType inputType) => this.BlockedInputType = inputType;
        public void SetAbilityBehaviour(AbilityBehaviour abilityBehaviour) => this.AbilityBehaviour = abilityBehaviour;
        public void SetCastClip(AnimationClip clip) => this.CastClip = clip;
        public void SetCooldown(float cooldown) => this.Cooldown = cooldown;
        

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
            OnStarted?.Invoke(InventorySlotID);
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
        
        public IAbility Build()
        {
            return ability;
        }
    }
}