using ScriptableObjects.Unit.Character.Creep;
using Zenject;

namespace Gameplay.Unit.Character.Creep
{
    public class BeholderHealth : CreepHealth, IPlayerAttackable, ITrapAttackable
    {
        [Inject] private DiContainer diContainer;
        
        public override void Initialize()
        {
            base.Initialize();

            var so_BeholderHealth = (SO_BeholderHealth)so_CreepHealth;
            
            var beholderController = (BeholderController)unitController;
            beholderController.BeholderStateFactory.SetBeholderHealthConfig(so_BeholderHealth);
            
            var takeDamageState = (BeholderTakeDamageState)beholderController.CreepStateFactory.CreateState(typeof(BeholderTakeDamageState));
            diContainer.Inject(takeDamageState);
            takeDamageState.Initialize();
            beholderController.StateMachine.AddStates(takeDamageState);
            
            beholderController.GetComponentInUnit<BeholderAnimation>().AddClip(so_BeholderHealth.takeDamageClip);
        }

        public override void TakeDamage(DamageData damageData)
        {
            base.TakeDamage(damageData);
            
            if(!isCanTakeDamageEffect) return;
            CreepController.StateMachine.ExitOtherStates(typeof(BeholderTakeDamageState), true);
        }
    }
}