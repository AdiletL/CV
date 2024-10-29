using UnityEngine;

public abstract class CharacterMainController : MonoBehaviour, ICharacter
{
    [field: SerializeField] public ComponentsInGameObjects components { get; private set; }
    
    public virtual void Initialize()
    {
        components.Initialize();
    }
}
