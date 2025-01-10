using System.Diagnostics;
using UnityEngine;

public class TestTime : MonoBehaviour
{
    
    void Start()
    {
        Stopwatch stopwatch = new Stopwatch();

        // Запускаем таймер
        stopwatch.Start();

        // Код, для которого измеряем время выполнения
        for (int i = 0; i < 1000000; i++)
        {
            Mathf.Pow(i, 2);
        }

        // Останавливаем таймер
        stopwatch.Stop();

        // Выводим время выполнения
        UnityEngine.Debug.Log($"Время выполнения: {stopwatch.ElapsedMilliseconds} ms");
    }
}
