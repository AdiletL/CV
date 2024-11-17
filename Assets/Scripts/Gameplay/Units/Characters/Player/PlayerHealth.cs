using Unit.Character;

namespace Unit.Character.Player
{
    public class PlayerHealth : CharacterHealth
    {
        public override UnitType UnitType { get; } = UnitType.player;
    }
}