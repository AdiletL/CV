using System;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform mainCamera;
    private Transform target;

    private void Start()
    {
        target = Camera.main.transform;
    }

    private void LateUpdate()
    {
        LookAt(target);
    }
    
    public void LookAt(Transform target)
    {
        var direction = target.position - transform.position;
        if (direction == Vector3.zero) return;

        var targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Euler(-targetRotation.eulerAngles.x, target.rotation.y, 0);
    }
}
