using Gameplay;
using Gameplay.Unit;
using UnityEngine;

namespace ScriptableObjects.Unit
{
    public abstract class SO_UnitExperience : ScriptableObject
    {
        [field: SerializeField] public int StartLevel { get; protected set; } = 1;
        [field: SerializeField] public int CurrentExperience { get; protected set; } = 0;
        [field: SerializeField] public int MaxExperience { get; protected set; } = 0;
        [field: SerializeField] public int RangeTakeExperience { get; protected set; }
        [field: SerializeField, Space(5)] public bool IsTakeLevel { get; protected set; }
        [field: SerializeField] public bool IsGiveExperience { get; protected set; }
        [field: SerializeField] public bool IsTakeExperience { get; protected set; }
        
        [field: SerializeField, Space(10)] public UnitStatConfigData IncreaseStatConfig { get; protected set; }
    }
}