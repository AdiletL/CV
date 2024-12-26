using System;
using Unit.Character.Player;
using UnityEngine;

namespace Unit.Reward
{
    public abstract class RewardController : UnitController
    {
        public override UnitType UnitType { get; } = UnitType.reward;
    }
}
