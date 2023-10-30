using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinAllLevel : MonoBehaviour
{
    public IEnumerator ShowWinAllLevelWindow(float timeScale = 0.2f)
    {
        this.transform.localScale = Vector3.zero;
        this.gameObject.SetActive(true);
        float elapsedTime = 0;
        bool reachedScale = false;
        while (!reachedScale)
        {
            if (Vector3.Distance(this.transform.localScale, Vector3.one) < 0.01f)
            {
                reachedScale = true;
                break;
            }
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / timeScale, 0f, 1f);
            MakeSmoothNum.SmootherStep(t);
            this.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
    }
}
