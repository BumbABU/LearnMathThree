using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnCoroutine : MonoBehaviour
{
    [SerializeField] private bool isCount = false;
    private Coroutine countMeat;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            this.isCount = !this.isCount;
            if (this.isCount)
            {
                Debug.Log("Start");
                this.countMeat = StartCoroutine(CountMeat());
            }
            else
            {
                Debug.Log("Stop");
                StopCoroutine(this.countMeat);
            }
        }
    }

    IEnumerator CountMeat()
    {
        int meat = 0;
        yield return new WaitForSeconds(5f);
        while (true)
        {
            meat++;
            Debug.Log("Meat: " + meat);
            yield return new WaitForSeconds(1f);
        }
    }
}
