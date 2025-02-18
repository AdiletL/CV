using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_GameHotkeys", menuName = "SO/Gameplay/Hotkeys", order = 51)]
    public class SO_GameHotkeys : ScriptableObject
    {
        [field: SerializeField] public int SelectObjectMouseButton { get; private set; } = 0;
        [field: SerializeField] public int SpecialActionMouseButton { get; private set; } = 1;
        [field: SerializeField] public int AttackMouseButton { get; private set; } = 1;
        
        
        [field: SerializeField, Header("Jump")] 
        public KeyCode JumpKey { get; private set; } = KeyCode.Space;
        
        
        [field: SerializeField, Header("Open Chest")] 
        public KeyCode OpenContainerKey { get; private set; } = KeyCode.E;
        
        
        [field: SerializeField, Header("Take Loot")] 
        public KeyCode TakeLootKey { get; private set; } = KeyCode.F;
        

        [field: SerializeField, Header("Skills")]
        public KeyCode DashKey { get; private set; } = KeyCode.LeftShift;
        
        
        [field: SerializeField, Header("SkillInventory")]
        public KeyCode[] AbilityInventoryKeys { get; private set; }

        [SerializeField] private int amountSkillInventoryKeys = 1;

        [Button]
        public void SetAmountSkillInventoryKeys()
        {
            AbilityInventoryKeys = new KeyCode[amountSkillInventoryKeys];
            for (int i = 0; i < amountSkillInventoryKeys; i++)
                AbilityInventoryKeys[i] = KeyCode.Alpha1 + i;
        }
    }
}