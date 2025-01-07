using Gameplay;
using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_Level_", menuName = "SO/Gameplay/Level/Level", order = 51)]
    public class SO_Level : ScriptableObject
    {
        [field: SerializeField] public GameFieldController[] GameFieldControllers { get; private set; }
    }
}