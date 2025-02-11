using DG.Tweening;
using Gameplay.Skill;
using Gameplay.UI;
using ScriptableObjects.Unit.Item;
using Unit.Character.Player;
using UnityEngine;

namespace Unit.Item
{
    public class ItemController : UnitController, IItem
    {
        [SerializeField] private SO_Item so_Item;
        private HotkeyUI hotkeyUI;

        private float jumpPower;
        private float jumpDuration;

        private bool isSelected;

        private ItemData itemData;
        
        public bool IsCanTake { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            itemData = new ItemData(so_Item.Name, so_Item.ItemType, so_Item.Icon, so_Item.Amount, so_Item.IsCanSelect, so_Item.SkillType);
            jumpPower = so_Item.JumpPower;
            jumpDuration = so_Item.JumpDuration;
            
            hotkeyUI = GetComponentInUnit<PressHotkeyUI>();
            Disable();
        }

        public override void Appear()
        {
            
        }

        public override void Disappear()
        {
            throw new System.NotImplementedException();
        }

        public void TakeItem(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out PlayerInventory playerInventory))
            {
                if (!playerInventory.IsFullInventory())
                {
                    playerInventory.AddItem(itemData);
                    Destroy(this.gameObject);
                }
            }
        }

        public void Enable(KeyCode takeKey)
        {
            isSelected = true;
            hotkeyUI.SetText(takeKey.ToString());
            hotkeyUI.Show();
        }

        public void Disable()
        {
            hotkeyUI.Hide();
            isSelected = false;
        }

        public void JumpToPoint(Vector3 point)
        {
            transform.DOJump(point, jumpPower, 1, jumpDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    IsCanTake = true;
                    if(isSelected) hotkeyUI.Show();
                });
        }
    }

    public enum ItemType
    {
        nothing,
        weapon,
        meat,
        plant,
    }

    public class ItemData
    {
        public string Name { get; private set; }
        public ItemType ItemType { get; private set; }
        public SkillType SkillType { get; private set; }
        public Sprite Icon { get; private set; }
        public int Amount { get; set; }
        public bool IsCanSelect { get; private set; }

        public ItemData(string name, ItemType itemType, Sprite icon, int amount, bool isCanSelect, SkillType skillType)
        {
            Name = name;
            ItemType = itemType;
            Icon = icon;
            Amount = amount;
            IsCanSelect = isCanSelect;
            this.SkillType = skillType;
        }
    }
}