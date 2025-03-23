namespace Gameplay.Unit.Character.Player
{
    public class PlayerItemUsageState : CharacterItemUsageState
    {
        private PlayerItemInventory playerInventory;
        
        public void SetPlayerInventory(PlayerItemInventory playerInventory) => this.playerInventory = playerInventory;

        protected override void FinishDurationAnimation()
        {
            playerInventory.FinishedCastItem(currentItem);
            base.FinishDurationAnimation();
        }
    }

    public class PlayerItemUsageStateBuilder : CharacterItemUsageStateBuilder
    {
        public PlayerItemUsageStateBuilder() : base(new PlayerItemUsageState())
        {
        }

        public PlayerItemUsageStateBuilder SetInventory(PlayerItemInventory playerInventory)
        {
            if(state is PlayerItemUsageState playerItemUsageState)
                playerItemUsageState.SetPlayerInventory(playerInventory);
            return this;
        }
    }
}