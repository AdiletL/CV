using System.Collections.Generic;
using Gameplay.Unit.Portal;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class LevelController : MonoBehaviour
    {
        [Inject] private DiContainer diContainer;

        [SerializeField] private PortalController startPortal;
        
        private List<RoomController> rooms = new();
        public int ID { get; private set; }

        public bool IsNullRoom(int roomID)
        {
            for (int i = rooms.Count - 1; i >= 0; i--)
            {
                if (rooms[i].ID == roomID)
                    return false;
            }
            return true;
        }
            
        public void Initialize()
        {
            diContainer.Inject(startPortal);
            startPortal.Initialize();
            
            PortalController[] portals = new PortalController[1] { startPortal };
            startPortal.Initialize();
            diContainer.Bind<PortalController[]>().FromInstance(portals).AsSingle();
        }
        
        public void SetID(int id) => ID = id;

        public void AddRoom(RoomController roomController)
        {
            for (int i = rooms.Count - 1; i >= 0; i--)
            {
                if(rooms[i].ID == roomController.ID) 
                    return;
            }
            rooms.Add(roomController);
        }

        public void RemoveRoom(int id)
        {
            for (int i = rooms.Count - 1; i >= 0; i--)
            {
                if (rooms[i].ID == id)
                {
                    rooms.Remove(rooms[i]);
                    return;
                }
            }
        }
        
    }
}