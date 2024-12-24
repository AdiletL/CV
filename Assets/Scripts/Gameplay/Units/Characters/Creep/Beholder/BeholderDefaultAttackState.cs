namespace Unit.Character.Creep
{
    public class BeholderDefaultAttackState : CreepDefaultAttackState
    {
        
    }
    
    public class BeholderDefaultAttackStateBuilder : CharacterDefaultAttackStateBuilder
    {
        public BeholderDefaultAttackStateBuilder() : base(new BeholderDefaultAttackState())
        {
        }
    }
}