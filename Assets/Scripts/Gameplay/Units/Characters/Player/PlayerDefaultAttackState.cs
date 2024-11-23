namespace Unit.Character.Player
{
    public class PlayerDefaultAttackState : CharacterDefaultAttackState
    {
        
    }
    
    public class PlayerDefaultAttackStateBuilder : CharacterDefaultAttackStateBuilder
    {
        public PlayerDefaultAttackStateBuilder() : base(new PlayerDefaultAttackState())
        {
        }
    }
}