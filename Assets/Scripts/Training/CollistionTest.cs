using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollistionTest : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
    }
    /*  private void FixedUpdate()
      {
          Debug.Log("FixUpdate");
      }

      private void Update()
      {
          Debug.Log("Update");
      }
      private void OnTriggerEnter2D(Collider2D collision)
      {
         // Debug.Log(collision.gameObject.name);
      }

      private void OnCollisionEnter2D(Collision2D collision)
      {
          Debug.Log("CollisionEnter2d");
      }

      private void OnCollisionStay2D(Collision2D collision)
      {
          Debug.Log("CollisionStay");
      }*/
}
