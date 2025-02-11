using Unity.Cinemachine;
using UnityEngine;

namespace Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera cinemachineCameraPrefab;

        public CinemachineCamera CurrentCinemachineCamera { get; private set; }
        
        public void Initialize()
        {
            CurrentCinemachineCamera = Instantiate(cinemachineCameraPrefab);
        }
    }
}