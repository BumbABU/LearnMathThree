using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycast : MonoBehaviour
{
    Ray2D ray;
    // Start is called before the first frame update
    void Start()
    {
        //ray = new Ray(transform.position, transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
       // ray = new Ray2D(transform.position, transform.forward);
        if ( Physics2D.Raycast(transform.position,Vector2.up)) {
            Debug.Log("hit");
        }
    }
}
