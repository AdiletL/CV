using Gameplay.Damage;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerSwitchAttackState : CharacterSwitchAttackState
    {
        private PlayerAnimation playerAnimation;
        private SO_PlayerAttack so_PlayerAttack;
        private float rangeAttack;
        
        protected bool isCheckDistanceToTarget(Vector3 targetPosition)
        {
            float distanceToTarget = Vector3.Distance(this.GameObject.transform.position, targetPosition);
            if (distanceToTarget > rangeAttack)
                return false;
            else
                return true;
        }
        

        public override void Initialize()
        {
            base.Initialize();
            playerAnimation = (PlayerAnimation)CharacterAnimation;
            so_PlayerAttack = (SO_PlayerAttack)SO_CharacterAttack;
            damageble = new NormalDamage(so_PlayerAttack.Damage);
            rangeAttack = so_PlayerAttack.RangeAttack;
        }

        protected override void DestermineState()
        {
            //TODO: Type attack state
            
            if(!attackStates.ContainsKey(typeof(PlayerMeleeAttackState)))
            {
                var meleeState = (PlayerMeleeAttackState)new PlayerMeleeAttackBuilder()
                    .SetEnemyLayer(EnemyLayer)
                    .SetCenter(Center)
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
        
        public bool IsCheckTarget()
        {
            Collider[] hits = Physics.OverlapSphere(Center.position, rangeAttack, EnemyLayer);

            if (hits.Length == 0) return false;

            if (!hits[0].TryGetComponent(out IHealth health) || !health.IsLive) return false;
            if (!isCheckDistanceToTarget(hits[0].transform.position)) return false;
            if (hits[0].TryGetComponent(out UnitCenter unitCenter))
            {
                var direcitonToTarget = (unitCenter.Center.position - Center.position).normalized;
                if (Physics.Raycast(Center.position, direcitonToTarget, out var hit, rangeAttack)
                    && hit.transform.gameObject == hits[0].gameObject)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class PlayerAttackStateBuilder : CharacterAttackStateBuilder
    {
        public PlayerAttackStateBuilder() : base(new PlayerSwitchAttackState())
        {
        }
        
    }
}