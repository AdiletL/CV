using System;
using Unit;
using Unit.Character;
using Unit.Character.Player;
using UnityEngine;

namespace Gameplay.Factory.Character
{
    public abstract class CharacterSwitchStateFactory : Factory
    {
        protected GameObject gameObject;
        protected UnitCenter unitCenter;
        
        public void SetUnitCenter(UnitCenter unitCenter) => this.unitCenter = unitCenter;
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;


        public abstract void Initialize();
        public abstract CharacterSwitchAttackState CreateSwitchAttackState(Type stateType);
        public abstract CharacterSwitchMoveState CreateSwitchMoveState(Type stateType);
    }

    public abstract class CharacterSwitchStateFactoryBuilder
    {
        protected CharacterSwitchStateFactory factory;

        public CharacterSwitchStateFactoryBuilder(CharacterSwitchStateFactory characterStateFactory)
        {
            this.factory = characterStateFactory;
        }

        public CharacterSwitchStateFactoryBuilder SetGameObject(GameObject gameObject)
        {
            this.factory.SetGameObject(gameObject);
            return this;
        }

        public CharacterSwitchStateFactoryBuilder SetUnitCenter(UnitCenter unitCenter)
        {
            this.factory.SetUnitCenter(unitCenter);
            return this;
        }

        public CharacterSwitchStateFactory Build()
        {
            return factory;
        }
    }
}