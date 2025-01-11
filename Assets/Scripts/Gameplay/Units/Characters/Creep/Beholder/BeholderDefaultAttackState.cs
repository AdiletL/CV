namespace Unit.Character.Creep
{
    public class BeholderDefaultAttackState : CreepDefaultAttackState
    {

        public override void Update()
        {
            if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, currentTarget.transform.position,
                    range * range))
            {
                this.StateMachine.GetState<BeholderSwitchMoveState>().SetTarget(currentTarget);
                this.StateMachine.ExitCategory(Category, typeof(BeholderSwitchMoveState));
                return;
            }
            base.Update();
        }
    }
    
    public class BeholderDefaultAttackStateBuilder : CharacterDefaultAttackStateBuilder
    {
        public BeholderDefaultAttackStateBuilder() : base(new BeholderDefaultAttackState())
        {
        }
    }
}