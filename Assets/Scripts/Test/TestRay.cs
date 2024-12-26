using UnityEngine;

public class TestRay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.DrawRay(transform.position,( transform.position + Vector3.forward) * 100, Color.yellow, 2);
        Debug.Log("asf");
        if (Physics.Raycast(transform.position, transform.position + Vector3.forward, out RaycastHit hit, 10))
        {
            Debug.Log(hit.collider.name);
        }
    }
}
