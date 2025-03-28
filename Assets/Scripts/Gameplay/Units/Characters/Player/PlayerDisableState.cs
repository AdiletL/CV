namespace Gameplay.Unit.Character.Player
{
    public class PlayerDisableState : CharacterDisableState
    {
        public override void ActivateDisable(DisableType disableType)
        {
            if((so_GameDisable.BlockActions[disableType] & DisableCategory.Control) != 0 && 
               gameObject.TryGetComponent(out IPlayerController playerController))
                playerController.DeactivateControl();
            base.ActivateDisable(disableType);
        }

        public override void DeactivateDisable(DisableType disableType)
        {
            base.DeactivateDisable(disableType);
            if(!IsActionBlocked(DisableCategory.Control) && 
               gameObject.TryGetComponent(out IPlayerController playerController))
                playerController.ActivateControl();
        }
    }
    
    public class PlayerDisableStateBuilder : CharacterDisableStateBuilder
    {
        public PlayerDisableStateBuilder() : base(new PlayerDisableState())
        {
        }
    }
}