using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public Vector3 targetPosition;  // Целевая позиция для движения
    public float moveSpeed = 5f;    // Скорость движения к цели
    public float jumpHeight = 2f;   // Максимальная высота прыжка
    public AnimationCurve jumpCurve; // Кривая для анимации прыжка

    private float jumpTime = 0f;    // Время для прыжка
    private bool isJumping = false; // Флаг, если объект прыгает
    private Vector3 startPosition;  // Начальная позиция для прыжка

    void Update()
    {
        // Двигаемся к цели
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Если объект ещё не прыгал, начинаем прыжок
        if (!isJumping)
        {
            StartJump();
        }

        // Если объект прыгает, применяем анимацию через AnimationCurve
        if (isJumping)
        {
            PerformJump();
        }
    }

    void StartJump()
    {
        isJumping = true;
        startPosition = transform.position;
        jumpTime = 0f; // Сбрасываем время для прыжка
    }

    void PerformJump()
    {
        // Проходим по времени с использованием кривой для высоты прыжка
        jumpTime += Time.deltaTime;
        float normalizedTime = jumpTime / 1f; // Время от 0 до 1 для кривой

        // Получаем высоту прыжка по кривой
        float jumpY = jumpCurve.Evaluate(normalizedTime) * jumpHeight;

        // Обновляем позицию с учётом высоты прыжка
        transform.position = new Vector3(transform.position.x, startPosition.y + jumpY, transform.position.z);

        // Когда прыжок завершен (время больше 1), заканчиваем прыжок
        if (normalizedTime >= 1f)
        {
            isJumping = false;
        }
    }
}