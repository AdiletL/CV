using System;
using Gameplay.Ability;
using ScriptableObjects.Ability;

namespace Gameplay.Factory
{
    public class AbilityFactory : Factory
    {
        public Ability.Ability CreateAbility(SO_Ability so_Ability)
        {
            Ability.Ability result = so_Ability.AbilityTypeID switch
            {
                _ when so_Ability.AbilityTypeID == AbilityType.Nothing => null,
                _ when so_Ability.AbilityTypeID == AbilityType.Dash => CreateDash(so_Ability),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return result;
        }

        private DashAbility CreateDash(SO_Ability so_Ability)
        {
            return new DashAbility(so_Ability as SO_DashAbility);
        }
    }
}