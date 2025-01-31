using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_Level_", menuName = "SO/Gameplay/Level/Level", order = 51)]
    public class SO_Level : ScriptableObject
    {
        [field: SerializeField] public GameFields[] GameFields { get; private set; }
        
    }

    [System.Serializable]
    public class GameFields
    {
        public AssetReferenceGameObject[] GameFieldControllers;
    }
}