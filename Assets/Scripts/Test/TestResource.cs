using UnityEngine;
using UnityEngine.Serialization;

namespace Test
{
    public enum ResourceType
    {
        Nothing,
        
    }
    public class TestResource : MonoBehaviour
    {
        [field: SerializeField] public ResourceType ResourceTypeID { get; private set; }
    }
}