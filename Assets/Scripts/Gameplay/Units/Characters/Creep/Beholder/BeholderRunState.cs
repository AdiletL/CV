using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderRunState : CreepRunState
    {
        private CharacterSwitchAttackState characterSwitchAttack;

        private float countCooldownCheckEnemy;

        private const float cooldownCheckEnemy = .3f;

        public void SetCharacterSwitchAttack(CharacterSwitchAttackState characterSwitchAttackState) =>
            this.characterSwitchAttack = characterSwitchAttackState;
        

        public override void Update()
        {
            base.Update();
            
            CheckEnemy();
        }

        private void CheckEnemy()
        {
            countCooldownCheckEnemy += Time.deltaTime;
            if (countCooldownCheckEnemy > cooldownCheckEnemy)
            {
                if (characterSwitchAttack.IsFindUnitInRange())
                    characterSwitchAttack.ExitCategory(Category);
                
                countCooldownCheckEnemy = 0;
            }
        }
    }
    
    public class BeholderRunStateBuilder : CreepRunStateBuilder
    {
        public BeholderRunStateBuilder() : base(new BeholderRunState())
        {
        }

        public BeholderRunStateBuilder SetCharacterSwitchAttack(CharacterSwitchAttackState characterSwitchAttackState)
        {
            if(state is BeholderRunState beholderRunState)
                beholderRunState.SetCharacterSwitchAttack(characterSwitchAttackState);
            return this;
        }
    }
}