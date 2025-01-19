using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_GameHotkeys", menuName = "SO/Gameplay/Hotkeys", order = 51)]
    public class SO_GameHotkeys : ScriptableObject
    {
        [field: SerializeField, Header("Attack Enemy")] 
        public KeyCode AttackKey { get; private set; } = KeyCode.A;

        [field: SerializeField] public int AttackMouseButton { get; private set; } = 0;
        
        [field: SerializeField, Header("Select Cell"), Space(10)] 
        public int SelectCellMouseButton { get; private set; } = 1;
        
        [field: SerializeField, Header("Jump"), Space(10)] 
        public KeyCode JumpKey { get; private set; } = KeyCode.Space;
        
        [field: SerializeField, Header("Open Chest"), Space(10)] 
        public KeyCode OpenChestKey { get; private set; } = KeyCode.E;

        [field: SerializeField, Header("Skills"), Space(10)]
        public KeyCode DashKey { get; private set; } = KeyCode.LeftShift;
    }
}