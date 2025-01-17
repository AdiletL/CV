using ScriptableObjects.Unit.Character.Player;
using Unit.Cell;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerControlDesktop : CharacterControlDesktop
    {
        private PlayerController playerController;
        private PlayerSwitchAttack playerSwitchAttack;
        private PlayerIdleState playerIdleState;
        private PlayerSwitchMove playerSwitchMove;
        private SO_PlayerControlDesktop so_PlayerControlDesktop;
        
        private RaycastHit[] hits = new RaycastHit[1];

        private GameObject previousHit;
        private Texture2D selectAttackTexture;
        private LayerMask enemyLayer;

        private bool isSelectedAttack;
        
        public PlayerControlDesktop(PlayerController playerController, PlayerSwitchAttack playerSwitchAttack, 
            PlayerSwitchMove playerSwitchMove, SO_PlayerControlDesktop so_PlayerControlDesktop, LayerMask enemyLayer)
        {
            this.playerController = playerController;
            this.so_PlayerControlDesktop = so_PlayerControlDesktop;
            this.playerSwitchMove = playerSwitchMove;
            this.playerSwitchAttack = playerSwitchAttack;
            this.enemyLayer = enemyLayer;
        }
        
        private bool tryGetHitPosition(out GameObject hitObject, LayerMask layerMask)
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            var hitsCount = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity, layerMask);
            for (int i = 0; i < hitsCount; i++)
            {
                hitObject = hits[i].transform.gameObject;
                return true;
            }

            hitObject = null;
            return false;
        }

        public override void Initialize()
        {
            base.Initialize();
            selectAttackTexture = so_PlayerControlDesktop.SelectAttackIcon.texture;
        }

        protected override void ClearHotkey()
        {
            base.ClearHotkey();
            isSelectedAttack = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public override void HandleHotkey()
        {
            base.HandleHotkey();
            if (Input.GetKeyDown(KeyCode.A))
            {
                isSelectedAttack = true;
                Cursor.SetCursor(selectAttackTexture, Vector2.zero, CursorMode.Auto);
            }
        }

        public override void HandleInput()
        {
            if (isSelectedAttack && Input.GetMouseButtonDown(0))
            {
                if (tryGetHitPosition(out GameObject hitObject, enemyLayer))
                {
                    playerSwitchAttack.SetTarget(hitObject);
                    playerSwitchAttack.ExitOtherStates();
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (tryGetHitPosition(out GameObject hitObject, Layers.CELL_LAYER))
                {
                    playerSwitchMove.SetTarget(hitObject);
                    playerSwitchMove.ExitOtherStates();
                }

                ClearHotkey();
            }
        }
    }
}