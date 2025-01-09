namespace Unit.Character.Player
{
    public class PlayerTakeDamageState : CharacterTakeDamageState
    {
        public override void Enter()
        {
            base.Enter();
            this.StateMachine.ExitCategory(Category, null);
        }
    }
    
    public class PlayerTakeDamageStateBuilder : CharacterTakeDamageStateBuilder
    {
        public PlayerTakeDamageStateBuilder() : base(new PlayerTakeDamageState())
        {
        }
    }
}