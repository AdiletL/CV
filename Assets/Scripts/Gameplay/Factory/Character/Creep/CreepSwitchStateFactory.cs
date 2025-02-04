using Unit.Character;
using UnityEngine.AI;

namespace Gameplay.Factory.Character.Creep
{
    public abstract class CreepSwitchStateFactory : CharacterSwitchStateFactory
    {
        protected NavMeshAgent navMeshAgent;
        protected StateMachine stateMachine;
        protected CharacterAnimation characterAnimation;
        protected CharacterEndurance characterEndurance;
        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetCharacterEndurance(CharacterEndurance characterEndurance) => this.characterEndurance = characterEndurance;
    }

    public abstract class CreepSwitchStateFactoryBuilder : CharacterSwitchStateFactoryBuilder
    {
        protected CreepSwitchStateFactoryBuilder(CharacterSwitchStateFactory characterStateFactory) : base(characterStateFactory)
        {
        }

        public CreepSwitchStateFactoryBuilder SetNavMeshAgent(NavMeshAgent navMeshAgent)
        {
            if(factory is CreepSwitchStateFactory creepStateFactory)
                creepStateFactory.SetNavMeshAgent(navMeshAgent);
            return this;
        }

        public CreepSwitchStateFactoryBuilder SetStateMachine(StateMachine stateMachine)
        {
            if(factory is CreepSwitchStateFactory creepStateFactory)
                creepStateFactory.SetStateMachine(stateMachine);
            return this;
        }
        
        public CreepSwitchStateFactoryBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if(factory is CreepSwitchStateFactory creepStateFactory)
                creepStateFactory.SetCharacterAnimation(characterAnimation);
            return this;
        }
        
        public CreepSwitchStateFactoryBuilder SetCharacterEndurance(CharacterEndurance characterEndurance)
        {
            if(factory is CreepSwitchStateFactory creepStateFactory)
                creepStateFactory.SetCharacterEndurance(characterEndurance);
            return this;
        }
    }
}