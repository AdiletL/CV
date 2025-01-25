using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerJumpState : CharacterJumpState
    {
        public IEndurance Endurance { get; set; }
        public KeyCode JumpKey { get; set; }
        public float JumpReductionEndurance { get; set; }
        

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
            ReductionEndurance();
        }

        
        private void ReductionEndurance()
        {
            Endurance.RemoveEndurance(JumpReductionEndurance);
        }
    }
    
    public class PlayerJumpStateBuilder : CharacterJumpStateBuilder
    {
        public PlayerJumpStateBuilder() : base(new PlayerJumpState())
        {
        }

        public PlayerJumpStateBuilder SetEndurance(IEndurance playerEndurance)
        {
            if(state is PlayerJumpState playerJumpState)
                playerJumpState.Endurance = playerEndurance;
            
            return this;
        }
        public PlayerJumpStateBuilder SetJumpKey(KeyCode jumpKey)
        {
            if(state is PlayerJumpState playerJumpState)
                playerJumpState.JumpKey = jumpKey;
            
            return this;
        }
        public PlayerJumpStateBuilder SetReductionEndurance(float jumpReductionEndurance)
        {
            if(state is PlayerJumpState playerJumpState)
                playerJumpState.JumpReductionEndurance = jumpReductionEndurance;
            
            return this;
        }
    }
}