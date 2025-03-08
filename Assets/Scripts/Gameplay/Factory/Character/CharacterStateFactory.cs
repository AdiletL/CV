using System;
using Gameplay.Unit;
using Machine;
using Unit;
using UnityEngine;

namespace Gameplay.Factory.Character
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
        protected CharacterStateFactory factory;

        public CharacterStateFactoryBuilder(CharacterStateFactory factory)
        {
            this.factory = factory;
        }

        public CharacterStateFactoryBuilder SetGameObject(GameObject gameObject)
        {
            this.factory.SetGameObject(gameObject);
            return this;
        }

        public CharacterStateFactoryBuilder SetUnitCenter(UnitCenter unitCenter)
        {
            this.factory.SetUnitCenter(unitCenter);
            return this;
        }

        public CharacterStateFactory Build()
        {
            return factory;
        }
    }
}