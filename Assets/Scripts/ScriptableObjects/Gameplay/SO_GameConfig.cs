using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_GameConfig", menuName = "SO/Gameplay/Config", order = 51)]
    public class SO_GameConfig : ScriptableObject
    {
        [field: SerializeField] public int Experience { get; private set; }
        [field: SerializeField] public float BaseWaitTimeTrap { get; private set; }
    }
}