using Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderIdleState : CreepIdleState
    { 
        private BeholderSwitchAttack beholderSwitchAttack;
        private BeholderSwitchMove beholderSwitchMove;
        
        private float checkEnemyCooldown = .03f;
        private float countCheckEnemyCooldown;
        
        private bool isCheckAttack = true;

        public override void Initialize()
        {
            base.Initialize();
            beholderSwitchAttack = (BeholderSwitchAttack)CharacterSwitchAttack;
            beholderSwitchMove = (BeholderSwitchMove)CharacterSwitchMove;
        }
        
        public override void Update()
        {
            base.Update();

            CheckAttack();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            CheckMove();
        }

        private void CheckMove()
        {
            if(!beholderSwitchMove.IsCanMovement() || !isActive) return;
            beholderSwitchMove.ExitCategory(Category);
        }

        private void CheckAttack()
        {
            if(!isCheckAttack || !isActive) return;
            
            countCheckEnemyCooldown += Time.deltaTime;
            if (countCheckEnemyCooldown > checkEnemyCooldown)
            {
                if (beholderSwitchAttack.IsFindUnitInRange())
                {
                    beholderSwitchAttack.ExitCategory(Category);
                }

                countCheckEnemyCooldown = 0;
            }
        }
    }

    public class BeholderIdleStateBuilder : CreepIdleStateBuilder
    {
        public BeholderIdleStateBuilder() : base(new BeholderIdleState())
        {
        }
        
    }
}