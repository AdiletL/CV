using DG.Tweening;
using Gameplay.Loot;
using Gameplay.UI;
using ScriptableObjects.Unit.Item;
using UnityEngine;

namespace Unit.Item.Loot
{
    public class LootController : ItemController, ILoot
    {
        private SO_Loot so_Loot;
        private HotkeyUI hotkeyUI;

        private float jumpPower;
        private float jumpDuration;

        private bool isActive;
        private bool isSelected;

        private LootData lootData;

        public override void Initialize()
        {
            base.Initialize();
            so_Loot = (SO_Loot)so_Item;
            lootData = new LootData(so_Loot.Name, so_Loot.LootType, so_Loot.Icon, so_Loot.Amount);
            jumpPower = so_Loot.JumpPower;
            jumpDuration = so_Loot.JumpDuration;
            
            hotkeyUI = GetComponentInUnit<PressHotkeyUI>();
            Disable();
        }

        public override void Appear()
        {
            
        }

        public void TakeLoot(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out LootHandler lootHandler))
                lootHandler.AddLoot(lootData);
            Destroy(this.gameObject);
        }

        public void Enable(KeyCode takeKey)
        {
            isSelected = true;
            hotkeyUI.SetText(takeKey.ToString());
            
            if(!isActive) return;
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
                    isActive = true;
                    if(isSelected) hotkeyUI.Show();
                });
        }
    }

    public enum LootType
    {
        nothing,
        weapon,
        meat,
        plant,
    }

    public class LootData
    {
        public string Name { get; set; }
        public LootType LootType { get; set; }
        public Sprite Icon { get; set; }
        public int Amount { get; set; }

        public LootData(string name, LootType lootType, Sprite icon, int amount)
        {
            Name = name;
            LootType = lootType;
            Icon = icon;
            Amount = amount;
        }
    }
}