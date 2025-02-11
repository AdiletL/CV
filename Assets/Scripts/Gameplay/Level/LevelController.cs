using Unit.Portal;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class LevelController : MonoBehaviour
    {
        [Inject] private DiContainer diContainer;

        [SerializeField] private PortalController startPortal;
        public RoomController CurrentRoom { get; private set; }
        
        public void Initialize()
        {
            diContainer.Inject(startPortal);
            startPortal.Initialize();
            
            PortalController[] portals = new PortalController[1] { startPortal };
            startPortal.Initialize();
            diContainer.Bind<PortalController[]>().FromInstance(portals).AsSingle();
        }

        public void SetGameField(RoomController roomController)
        {
            CurrentRoom = roomController;
        }
    }
}