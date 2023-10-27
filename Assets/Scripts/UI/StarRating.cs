using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRating : MonoBehaviour
{
    public GameObject[] RateStars = new GameObject[3];
    public RectXformMover RectXformMover;

    public IEnumerator RatingStarRoutine(int starNum, float timeScale)
    {
        foreach (GameObject star in RateStars)
        {
            if (star != null)
            {
                star.transform.localScale = Vector3.zero;
            }
        }
        if(this.RectXformMover != null)
        {
            while(this.RectXformMover.IsMoving)
            {
                yield return null;
            }
        }
        for (int i = 0; i < starNum; i++)
        {
            if (RateStars[i] != null)
            {
                yield return StartCoroutine(ScaleStar(RateStars[i], timeScale));
            }
        }

    }

    public IEnumerator ScaleStar(GameObject star, float timeScale)
    {
        star.transform.localScale = Vector3.zero;
        bool reachedScale = false;
        float elapsedTime = 0;
        while (!reachedScale)
        {
            if (Vector3.Distance(star.transform.localScale, Vector3.one) < 0.01)
            {
                reachedScale = true;
                star.transform.localScale = Vector3.one;
                break;
            }
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / timeScale, 0, 1);
            MakeSmoothNum.SmootherStep(t);
            star.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
    }
}
