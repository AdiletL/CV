using System;

namespace Unit
{
    [Flags]
    public enum EntityType
    {
        nothing = 0,
        human = 1 << 0,
        monster = 1 << 1,
        weapon = 1 << 2,
        plant = 1 << 3,
        meat = 1 << 4,
    }
    public interface IState
    {
        public string Name { get; }
        public EntityType EntityType { get; }
    }
}
