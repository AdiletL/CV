using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_GameHotkeys", menuName = "SO/Gameplay/Hotkeys", order = 51)]
    public class SO_GameHotkeys : ScriptableObject
    {
        [field: SerializeField] public int SelectObjectMouseButton { get; private set; } = 0;
        
        [field: SerializeField, Header("Attack Enemy")] 
        public KeyCode AttackKey { get; private set; } = KeyCode.A;

        [field: SerializeField] public int AttackMouseButton { get; private set; } = 0;
        
        [field: SerializeField, Header("Select Cell")] 
        public int SelectCellMouseButton { get; private set; } = 1;
        
        [field: SerializeField, Header("Jump")] 
        public KeyCode JumpKey { get; private set; } = KeyCode.Space;
        
        [field: SerializeField, Header("Open Chest")] 
        public KeyCode OpenContainerKey { get; private set; } = KeyCode.E;
        
        [field: SerializeField, Header("Take Loot")] 
        public KeyCode TakeLootKey { get; private set; } = KeyCode.F;

        [field: SerializeField, Header("Skills")]
        public KeyCode DashKey { get; private set; } = KeyCode.LeftShift;
    }
}