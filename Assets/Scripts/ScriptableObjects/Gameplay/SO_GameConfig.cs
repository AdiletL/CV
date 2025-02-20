using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_GameConfig", menuName = "SO/Gameplay/Config", order = 51)]
    public class SO_GameConfig : ScriptableObject
    {
        [field: SerializeField] public SO_GameUIColor SO_GameUIColor { get; private set; }
        [field: SerializeField] public SO_GameHotkeys SO_GameHotkeys { get; private set; }
        
        [field: SerializeField, Space(20)] public float CooldownReductionEndurance { get; private set; } = .2f;
        [field: SerializeField] public float AmountReductionEndurance { get; private set; } = .01f;
        [field: SerializeField] public float BaseWaitTimeTrap { get; private set; }
        [field: SerializeField, Space(20)] public Material HighlightedMaterial { get; private set; }
        [field: SerializeField, Space] public Texture2D BaseCursor { get; private set; }
        [field: SerializeField, Header("Scale cell / 3")] public float RadiusCell { get; private set; } = .3f;
    }
}