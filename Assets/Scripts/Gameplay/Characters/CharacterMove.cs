using UnityEngine;
using Character;
using UnityEngine.Serialization;

public abstract class CharacterMove : MonoBehaviour, ICharacter, IMove
{
    [SerializeField] protected CharacterMainController characterMainController;
    [SerializeField] protected SO_CharacterMove config;
    protected StateMachine stateMachine;

    protected IdleState idleState;

    public virtual void Initialize()
    {
        stateMachine = new StateMachine();
        
        idleState = new IdleStateBuilder()
            .SetCharacters(this, characterMainController.components.GetComponentInGameObjects<CharacterAnimation>())
            .SetIdleClip(config.IdleClip)
            .Build();
        
        stateMachine.SetState(idleState);
    }

    protected void ChangeState(IState newState)
    {
        stateMachine.SetState(newState);
    }
}
