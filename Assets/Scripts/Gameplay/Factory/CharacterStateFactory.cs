using System;
using Machine;
using Unit;
using UnityEngine;

namespace Gameplay.Factory
{
    public abstract class CharacterStateFactory : Factory
    {
        protected GameObject gameObject;
        protected UnitCenter unitCenter;
        
        public void SetUnitCenter(UnitCenter unitCenter) => this.unitCenter = unitCenter;
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        
        public abstract void Initialize();

        public abstract State CreateState(Type stateType);
    }

    public abstract class CharacterStateFactoryBuilder
    {
        protected CharacterStateFactory characterStateFactory;

        public CharacterStateFactoryBuilder(CharacterStateFactory characterStateFactory)
        {
            this.characterStateFactory = characterStateFactory;
        }

        public CharacterStateFactoryBuilder SetGameObject(GameObject gameObject)
        {
            this.characterStateFactory.SetGameObject(gameObject);
            return this;
        }

        public CharacterStateFactoryBuilder SetUnitCenter(UnitCenter unitCenter)
        {
            this.characterStateFactory.SetUnitCenter(unitCenter);
            return this;
        }

        public CharacterStateFactory Build()
        {
            return characterStateFactory;
        }
    }
}