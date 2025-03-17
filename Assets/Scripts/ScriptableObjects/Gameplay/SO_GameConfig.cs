using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_GameConfig", menuName = "SO/Gameplay/Config", order = 51)]
    public class SO_GameConfig : ScriptableObject
    {
        [field: SerializeField] public SO_GameUIColor SO_GameUIColor { get; private set; }
        [field: SerializeField] public SO_GameStatIcon SO_GameStatIcon { get; private set; }
        [field: SerializeField] public SO_GameHotkeys SO_GameHotkeys { get; private set; }
        
        [field: SerializeField, Header("Rate in second 2 endurance = 1")] public float BaseConsumptionEnduranceRate { get; private set; } = .1f;
        [field: SerializeField] public float BaseWaitTimeTrap { get; private set; }
        [field: SerializeField, Space(20)] public Material HighlightedMaterial { get; private set; }
        [field: SerializeField, Space] public Texture2D BaseCursor { get; private set; }
        [field: SerializeField, Header("Scale cell / 3")] public float RadiusCell { get; private set; } = .3f;
    }
}