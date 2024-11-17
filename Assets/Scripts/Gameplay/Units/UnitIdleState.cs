using Calculate;
using Machine;
using UnityEngine;

namespace Unit
{
    public class UnitIdleState : State
    {
        public GameObject GameObject { get; set; }

        private readonly Vector3 rayFindPlatformPosition = Vector3.up * .5f;
        
        public override void Initialize()
        {
        }

        public override void Enter()
        {
            FindPlatform.GetPlatform(GameObject.transform.position + rayFindPlatformPosition, Vector3.down)?.AddGameObject(GameObject);
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
            FindPlatform.GetPlatform(GameObject.transform.position + rayFindPlatformPosition, Vector3.down)?.RemoveGameObject(GameObject);
        }
    }

    public class UnitIdleStateBuilder : StateBuilder<UnitIdleState>
    {
        public UnitIdleStateBuilder(UnitIdleState instance) : base(instance)
        {
        }

        public UnitIdleStateBuilder SetGameObject(GameObject gameObject)
        {
            state.GameObject = gameObject;
            return this;
        }
    }
}