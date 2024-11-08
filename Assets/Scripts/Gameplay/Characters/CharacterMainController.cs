using System;
using Gameplay.Experience;
using ScriptableObjects.Character;
using Unit;
using UnityEngine;

namespace Character
{
    public abstract class CharacterMainController : MonoBehaviour, ICharacterController
    {
        [field: SerializeField] public ComponentsInGameObjects components { get; protected set; }
        
        protected StateMachine stateMachine;
        protected IExperience experienceCalculate;

        public int Level { get; protected set; }
        public int Experience { get; protected set; }
        
        public T GetState<T>() where T : Machine.IState
        {
            return stateMachine.GetState<T>();
        }
        
        
        
        public virtual void Initialize()
        {
            experienceCalculate = new LinearExperience();
            components.Initialize();
            components.GetComponentInGameObjects<CharacterUI>()?.Initialize();
        }


        public virtual void IncreaseStates(params IState[] stats)
        {
            foreach (IState state in stats)
            {
            Debug.Log(state.StateType);
                switch (state.StateType)
                {
                    case StateType.nothing:
                        break;
                    case StateType.health:
                        components.GetComponentInGameObjects<CharacterHealth>()?.IncreaseStates(state);
                        break;
                    case StateType.movement:
                        stateMachine.GetState<CharacterMoveState>()?.IncreaseStates(state);
                        break;
                    case StateType.attack:
                        stateMachine.GetState<CharacterAttackState>()?.IncreaseStates(state);
                        break;
                    case StateType.level:
                        IncreaseLevel();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void AddExperience(int experience)
        {
            Experience += experience;
            CheckLevelUp();
        }
        private void CheckLevelUp()
        {
            int experienceToNextLevel = experienceCalculate.CalculateExperienceForNextLevel(Level);
        
            while (Experience >= experienceToNextLevel)
            {
                Experience -= experienceToNextLevel;
                LevelUp();
                experienceToNextLevel = experienceCalculate.CalculateExperienceForNextLevel(Level);
            }
        }
        public void LevelUp()
        {
            Level++;
        }

        public void IncreaseLevel()
        {
            LevelUp();
            Experience = experienceCalculate.CalculateExperienceForNextLevel(Level);
        }
    }
}
