using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreWindow : MonoBehaviour
{
    private bool _isCanToch = true;
    private bool _isShowing = false;
    public TextMeshProUGUI HighScoreText;

    public IEnumerator ShowHighScoreWindowRoutine(float timeScale = 0.2f)
    {
        if (this._isCanToch)
        {
            this._isShowing = !this._isShowing;
            if (this._isShowing)
            {
                this.SetHighScoreText();
                this.gameObject.SetActive(true);
                yield return null;
                yield return StartCoroutine(ScaleHighScoreWindowRoutine(Vector3.zero, Vector3.one, timeScale));
                Time.timeScale = 0;
            }
            else if (!this._isShowing)
            {
                Time.timeScale = 1;
                yield return StartCoroutine(ScaleHighScoreWindowRoutine(Vector3.one, Vector3.zero, timeScale));
                this.gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator ScaleHighScoreWindowRoutine(Vector3 startScale, Vector3 desScale, float timeScale = 0.2f)
    {
        this._isCanToch = false;
        this.transform.localScale = startScale;
        if (desScale == Vector3.one && startScale == Vector3.zero)
        {
            this.gameObject.SetActive(true);
        }
        float elapsedTime = 0;
        bool reachedScale = false;
        while (!reachedScale)
        {
            if (Vector3.Distance(this.transform.localScale, desScale) < 0.01f)
            {
                reachedScale = true;
                break;
            }
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / timeScale, 0f, 1f);
            MakeSmoothNum.SmootherStep(t);
            this.transform.localScale = Vector3.Lerp(startScale, desScale, t);
            yield return null;
        }
        this._isCanToch = true;
        if (desScale == Vector3.zero && startScale == Vector3.one)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void SetHighScoreText()
    {
        if (this.HighScoreText)
        {
            if (ScoreManager.Instance)
            {
                this.HighScoreText.text = ScoreManager.Instance.GetHighScore();
            }
        }
    }
}
