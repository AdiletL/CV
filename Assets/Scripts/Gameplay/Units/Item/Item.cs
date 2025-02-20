using System;
using System.Collections.Generic;
using Unit.Character.Player;
using UnityEngine;

namespace Gameplay.Units.Item
{
    public abstract class Item : IItem
    {
        public event Action<int?> OnActivated;
        public event Action<int?> OnStarted;
        public event Action<int?> OnFinished;
        public event Action<int?> OnExit;
        public event Action<int?, float, float> OnCountCooldown;
        
        public int? InventorySlotID { get; protected set; }
        public GameObject GameObject { get; protected set; }
        public AnimationClip CastClip { get; protected set; }
        public abstract ItemName ItemNameID { get; protected set; }
        public ItemCategory ItemCategoryID { get; protected set; }
        public ItemBehaviour ItemBehaviourID { get; protected set; }
        public InputType BlockInputType { get; protected set; }
        public Action FinishedCallBack { get; protected set; }
        public int Amount { get; protected set; }
        public float Cooldown { get; protected set; }
        public bool IsCooldown { get; protected set; }
        public List<Ability.Ability> Abilities { get; protected set; }

        private float countCooldown;
        protected bool isActivated;
        

        public void SetInventorySlotID(int? slotID) => InventorySlotID = slotID;
        public void SetGameObject(GameObject gameObject) => this.GameObject = gameObject;
        public void SetItemCategory(ItemCategory itemCategory) => ItemCategoryID = itemCategory;
        public void SetItemBehaviour(ItemBehaviour itemBehaviour) => ItemBehaviourID = itemBehaviour;
        public void SetCastClip(AnimationClip clip) => this.CastClip = clip;
        public void SetCooldown(float cooldown) => this.Cooldown = cooldown;
        public void SetAmountItem(int amount) => Amount = amount;
        public void SetBlockInputType(InputType inputType) => BlockInputType = inputType;
        

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

            if ((ItemBehaviourID & ItemBehaviour.Hidden) != 0)
            {
                Debug.Log($"{ItemBehaviourID} скрыто и не может быть использовано.");
                Exit();
                return;
            }
            
            FinishedCallBack = finishedCallBack;
            isActivated = true;
            OnActivated?.Invoke(InventorySlotID);
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

        public void StartEffect()
        {
            OnStarted?.Invoke(InventorySlotID);
            StartCooldown();
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
            FinishedCallBack?.Invoke();
            OnFinished?.Invoke(InventorySlotID);
            Exit();
        }
        public virtual void Exit()
        {
            SetCursor(null);
            isActivated = false;
            OnExit?.Invoke(InventorySlotID);
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
            item.SetGameObject(gameObject);
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

        public Item Build()
        {
            return item;
        }
    }
}