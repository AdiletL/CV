using Unit.Cell;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerControlDesktop : CharacterControlDesktop
    {
        private PlayerController playerController;
        
        private RaycastHit[] hits = new RaycastHit[1];

        private GameObject previousHit;
        
        public PlayerControlDesktop(PlayerController playerController)
        {
            this.playerController = playerController;
        }
        
        private bool tryGetHitPosition(out GameObject hitObject)
        {
            // Получаем позицию курсора или касания
            Vector3 mousePosition = Input.mousePosition;

            // Генерируем луч из камеры
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            var hitsCount = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity);

            for (int i = 0; i < hitsCount; i++)
            {
                hitObject = hits[i].transform.gameObject;
                return true;
            }

            // Если ничего не найдено
            hitObject = null;
            return false;
        }
        
        public override void HandleInput()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (tryGetHitPosition(out GameObject hitObject) && hitObject.TryGetComponent(out CellController cellController))
                {
                    playerController.GetState<PlayerIdleState>().SetTarget(hitObject);
                    playerController.StateMachine.ExitOtherStates(typeof(PlayerIdleState));
                    
                    previousHit?.GetComponent<UnitRenderer>()?.ResetColor();
                    previousHit = hitObject;
                    
                    var unitRenderer = hitObject.GetComponent<UnitRenderer>();
                    unitRenderer?.SetColor(Color.red);
                }
            }
        }
    }
}