using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Lerp : MonoBehaviour
{
    public Transform target;
    public float speed;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, speed*Time.deltaTime);
    }

}
