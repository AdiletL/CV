using Gameplay.Damage;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerSwitchAttackState : CharacterSwitchAttackState
    {
        private PlayerAnimation playerAnimation;
        private SO_PlayerAttack so_PlayerAttack;

        public override void Initialize()
        {
            base.Initialize();
            playerAnimation = (PlayerAnimation)CharacterAnimation;
            so_PlayerAttack = (SO_PlayerAttack)SO_CharacterAttack;
            damageble = new NormalDamage(so_PlayerAttack.Damage);
        }

        protected override void DestermineState()
        {
            //TODO: Type attack state
            
            if(!attackStates.ContainsKey(typeof(PlayerMeleeAttackState)))
            {
                var meleeState = (PlayerMeleeAttackState)new PlayerMeleeAttackBuilder()
                    .SetRangeAttack(so_PlayerAttack.RangeAttack)
                    .SetGameObject(this.GameObject)
                    .SetAmountAttack(so_PlayerAttack.AmountAttack)
                    .SetCharacterAnimation(playerAnimation)
                    .SetAttackClip(so_PlayerAttack.MeleeAttackClip)
                    .SetCooldowClip(so_PlayerAttack.CooldownMeleeAttackClip)
                    .SetDamageble(damageble)
                    .SetStateMachine(this.StateMachine)
                    .Build();
                
                meleeState.Initialize();
                attackStates.TryAdd(typeof(PlayerMeleeAttackState), meleeState);
                this.StateMachine.AddStates(meleeState);
            }
            
            this.StateMachine.SetStates(typeof(PlayerMeleeAttackState));
        }

    }

    public class PlayerAttackStateBuilder : CharacterAttackStateBuilder
    {
        public PlayerAttackStateBuilder() : base(new PlayerSwitchAttackState())
        {
        }
        
    }
}