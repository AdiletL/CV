using System;
using Photon.Pun;
using Unit.Character;
using Unit.Character.Player;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private GameObject target;
    
    private Vector3 directionPosition;

    private Vector3 currentPosition;

    private float startPositionY;
    
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = new Vector3(position.x, startPositionY, position.z);
    }

    private void Start()
    {
        startPositionY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            currentPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z - 2);
            transform.position = Vector3.Lerp(transform.position, currentPosition, 5 * Time.deltaTime);
        }
    }
}
