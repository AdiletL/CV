using System.Collections.Generic;
using Gameplay.Unit.Character.Player;
using KinematicCharacterController;
using ScriptableObjects.Unit.Portal;
using UnityEngine;

namespace Gameplay.Unit.Portal
{
    public class PortalController : UnitController
    {
        [field: SerializeField] public Transform SpawnPoint { get; private set; }

        [SerializeField] private SO_Portal so_Portal;
        [SerializeField] private ParticleSystem portalParticle;
        [SerializeField] private bool isBilateral = true;
        
        public string ID => so_Portal.ID;

        private PortalController endPortal;
        
        private List<PlayerController> players = new(5);

        public bool IsPlayerContains(PlayerController player)
        {
            return players.Contains(player);
        }
        
        public override void Appear()
        {
            portalParticle.Play();
        }

        public override void Disappear()
        {
            portalParticle.Stop();
        }

        public void SetEndPortal(PortalController endPortal)
        {
            this.endPortal = endPortal;
        }

        public void Teleport(PlayerController player)
        {
            players.Add(player);
            var motor = player.GetComponent<KinematicCharacterMotor>();
            if (motor != null)
            {
                motor.SetPositionAndRotation(SpawnPoint.position, SpawnPoint.rotation);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                if(!isBilateral || endPortal == null) return;
                if(endPortal.IsPlayerContains(player)) return;
                endPortal.Teleport(player);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                if(players.Contains(player))
                    players.Remove(player);
            }
        }
    }
}