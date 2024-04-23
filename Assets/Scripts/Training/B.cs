using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class B : MonoBehaviour
{
    public void De()
    {
        if (this != null)
        {
            Debug.Log(this.gameObject.name);
        }
    }
}
