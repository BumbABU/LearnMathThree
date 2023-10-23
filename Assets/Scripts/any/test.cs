using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class test : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log("Hello");
    }

    private void OnMouseEnter()
    {
        Debug.Log(this.gameObject.name);
    }
}
