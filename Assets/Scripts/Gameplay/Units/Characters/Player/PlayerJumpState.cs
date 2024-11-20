namespace Unit.Character.Player
{
    public class PlayerJumpState : CharacterJumpState
    {
        
    }
    
    public class PlayerJumpStateBuilder : CharacterJumpStateBuilder
    {
        public PlayerJumpStateBuilder() : base(new PlayerJumpState())
        {
        }
    }
}