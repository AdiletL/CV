using ScriptableObjects.Unit.Character.Player;
using Zenject;

namespace Gameplay.Unit.Character.Player
{
    public class PlayerDisableController : CharacterDisableController
    {
        [Inject] private DiContainer diContainer;
        
        private PlayerController playerController;
        
        public override void Initialize()
        {
            base.Initialize();
            playerController = (PlayerController)characterMainController;
            playerController.PlayerStateFactory.SetPlayerDisableConfig((SO_PlayerDisable)so_CharacterDisable);
            var disableState = playerController.PlayerStateFactory.CreateState(typeof(PlayerDisableState));
            diContainer.Inject(disableState);
            disableState.Initialize();
            playerController.StateMachine.AddStates(disableState);
        }
    }
}