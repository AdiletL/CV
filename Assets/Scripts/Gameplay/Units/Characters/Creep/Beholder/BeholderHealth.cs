using ScriptableObjects.Unit.Character.Creep;

namespace Unit.Character.Creep
{
    public class BeholderHealth : CreepHealth, IPlayerAttackable, ITrapAttackable
    {
        public override void Initialize()
        {
            base.Initialize();

            var so_BeholderHealth = (SO_BeholderHealth)so_CreepHealth;
            
            var beholderController = (BeholderController)unitController;
            beholderController.BeholderStateFactory.SetBeholderHealthConfig(so_BeholderHealth);
            
            var takeDamageState = (BeholderTakeDamageState)beholderController.CreepStateFactory.CreateState(typeof(BeholderTakeDamageState));
            beholderController.StateMachine.AddStates(takeDamageState);
        }

        public override void TakeDamage(IDamageable damageable)
        {
            base.TakeDamage(damageable);
            
            if(!isCanTakeDamageEffect) return;
            CreepController.StateMachine.ExitOtherStates(typeof(BeholderTakeDamageState), true);
        }
    }
}