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

    
    public void SetTarget(GameObject target)
    {
        this.target = target;
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
