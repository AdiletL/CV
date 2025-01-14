using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_GameConfig", menuName = "SO/Gameplay/Config", order = 51)]
    public class SO_GameConfig : ScriptableObject
    {
        [field: SerializeField] public float CooldownDecreaseEndurance { get; private set; } = .2f;
        [field: SerializeField] public float AmountDecreaseEndurance { get; private set; } = .01f;
        [field: SerializeField] public float BaseWaitTimeTrap { get; private set; }
    }
}