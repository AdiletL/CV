using System;
using System.Collections;
using Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Ability
{
    public abstract class Ability : IAbility
    {
        [Inject] protected DiContainer diContainer;
        
        public event Action<int?, float, float> OnCountCooldown;
        
        public int? SlotID { get; protected set; }
        public GameObject GameObject { get; protected set; }
        public abstract AbilityType AbilityType { get; protected set; }
        public AbilityBehaviour AbilityBehaviour { get; protected set; }
        public InputType BlockedInputType { get; protected set; }
        public AbilityType BlockedAbilityType { get; protected set; }
        public Action FinishedCallBack { get; protected set; }
        public AnimationClip CastClip { get; protected set; }
        public float Cooldown { get; protected set; }
        public bool IsCooldown { get; protected set; }

        private float countCooldown;
        protected bool isActivated;
        

        public void SetSlotID(int? slotID) => SlotID = slotID;
        public void SetGameObject(GameObject gameObject) => this.GameObject = gameObject;
        public void SetBlockedInputType(InputType inputType) => this.BlockedInputType = inputType;
        public void SetAbilityBehaviour(AbilityBehaviour abilityBehaviour) => this.AbilityBehaviour = abilityBehaviour;
        public void SetCastClip(AnimationClip clip) => this.CastClip = clip;
        public void SetCooldown(float cooldown) => this.Cooldown = cooldown;
        

        public virtual void Initialize()
        {
        }

        public virtual void Activate(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
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
            if (Cooldown > 0)
            {
                IsCooldown = true;
                countCooldown = Cooldown;
            }
        }

        public virtual void Update()
        {
            if (IsCooldown)
            {
                countCooldown -= Time.deltaTime;
                if (countCooldown <= 0)
                    IsCooldown = false;
                OnCountCooldown?.Invoke(SlotID, countCooldown, Cooldown);
            }
        }

        public virtual void LateUpdate()
        {
            
        }
        
        public virtual void Finish()
        {
            isActivated = false;
            FinishedCallBack?.Invoke();
            Exit();
        }
        public virtual void Exit()
        {
            
        }
    }

    public abstract class AbilityConfig
    {
        public Sprite Icon;
        public AbilityType AbilityType;
        public AbilityBehaviour AbilityBehaviour;
        public InputType BlockedInputType;
        public float Cooldown;
    }

    public class AbilityData
    {
        public int? SlotID { get; private set; }
        public AbilityType AbilityType { get; private set; }
        public AbilityBehaviour AbilityBehaviour { get; private set; }
        public Sprite Icon { get; private set; }

        public AbilityData(int? slotID, AbilityType abilityType, AbilityBehaviour abilityBehaviour, Sprite icon)
        {
            SlotID = slotID;
            AbilityType = abilityType;
            AbilityBehaviour = abilityBehaviour;
            Icon = icon;
        }
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