using System;
using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_LevelContainer", menuName = "SO/Gameplay/Level/Container", order = 51)]
    public class SO_LevelContainer : ScriptableObject
    {
        [field: SerializeField] public SO_Level[] Levels { get; private set; }
    }
}