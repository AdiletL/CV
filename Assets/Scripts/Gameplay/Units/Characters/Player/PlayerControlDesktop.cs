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

            previousHit?.GetComponent<UnitMeshRenderer>().ResetColor();
            for (int i = 0; i < hitsCount; i++)
            {
                // Возвращаем данные о позиции и объекте
                hitObject = hits[i].collider.gameObject;
                previousHit = hitObject;
                hitObject.GetComponent<UnitMeshRenderer>().SetColor(Color.red);
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
                if (tryGetHitPosition(out GameObject hitObject))
                {
                    playerController.GetState<PlayerIdleState>().SetTarget(hitObject);
                }
            }
        }
    }
}