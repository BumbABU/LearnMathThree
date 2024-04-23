using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C : B
{
    // Start is called before the first frame update
    void Start()
    {
        De();
    }

    public void changeBgm(float volume)
    {
        Debug.Log(volume);
    }
}
