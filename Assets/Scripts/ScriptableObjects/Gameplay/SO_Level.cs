using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_Level_", menuName = "SO/Gameplay/Level/Level", order = 51)]
    public class SO_Level : ScriptableObject
    {
        [field: SerializeField] public SO_Room StartRoom { get; private set; }
        [field: SerializeField] public SO_Room[] Rooms { get; private set; }
    }
}