using Character;
using Character.Player;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject target;
    
    private Vector3 directionPosition;

    private Vector3 currentPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            var playerController = FindFirstObjectByType<PlayerController>();
            if(playerController != null)
                target = playerController.gameObject;
        }
        else
        {
            currentPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z - 3);
            transform.position = Vector3.Lerp(transform.position, currentPosition, 5 * Time.deltaTime);
            
        }
    }
}
