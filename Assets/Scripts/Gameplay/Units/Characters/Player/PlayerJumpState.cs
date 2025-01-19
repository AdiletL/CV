using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerJumpState : CharacterJumpState
    {
        public PlayerEndurance PlayerEndurance { get; set; }
        public KeyCode JumpKey { get; set; }
        public float JumpDecreaseEndurance { get; set; }
        

        public override void Update()
        {
            base.Update();

            if (countCooldownCheckGround > cooldownCheckGround)
            {
                if (Input.GetKeyDown(JumpKey) && currentJumpCount < MaxJumpCount)
                {
                    StartJump();
                }
            }
        }

        protected override void StartJump()
        {
            base.StartJump();
            DecreaseEndurance();
        }

        
        private void DecreaseEndurance()
        {
            PlayerEndurance.RemoveEndurance(JumpDecreaseEndurance);
        }
    }
    
    public class PlayerJumpStateBuilder : CharacterJumpStateBuilder
    {
        public PlayerJumpStateBuilder() : base(new PlayerJumpState())
        {
        }

        public PlayerJumpStateBuilder SetPlayerEndurance(PlayerEndurance playerEndurance)
        {
            if(state is PlayerJumpState playerJumpState)
                playerJumpState.PlayerEndurance = playerEndurance;
            
            return this;
        }
        public PlayerJumpStateBuilder SetJumpKey(KeyCode jumpKey)
        {
            if(state is PlayerJumpState playerJumpState)
                playerJumpState.JumpKey = jumpKey;
            
            return this;
        }
        public PlayerJumpStateBuilder SetDecreaseEndurance(float jumpDecreaseEndurance)
        {
            if(state is PlayerJumpState playerJumpState)
                playerJumpState.JumpDecreaseEndurance = jumpDecreaseEndurance;
            
            return this;
        }
    }
}