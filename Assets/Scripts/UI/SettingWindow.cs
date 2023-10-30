using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : MonoBehaviour
{
    private bool _isCanToch = true;
    private bool _isShowing = false;
    public Slider BGM_Slider;
    public Slider SE_Slider;

    private void Start()
    {
        if (AudioManager.Instance)
        {
            if (this.BGM_Slider != null)
            {
                this.BGM_Slider.value = AudioManager.Instance.AttachBGMSource.volume;
            }

            if (this.SE_Slider != null)
            {
                this.SE_Slider.value = AudioManager.Instance.AttachSESource.volume;
            }
        }
    }

    public void ChangeBGMVol()
    {
        if (AudioManager.Instance)
        {
            AudioManager.Instance.ChangeBGMVolume(this.BGM_Slider.value);
        }
    }

    public void ChangeSEVol()
    {
        if(AudioManager.Instance)
        {
            AudioManager.Instance.ChangeSEVolume(this.SE_Slider.value);
        }
    }

    public IEnumerator ShowSettingWindowRoutine(float timeScale = 0.2f)
    {
        if (this._isCanToch)
        {
            this._isShowing = !this._isShowing;
            if (this._isShowing)
            {
                this.gameObject.SetActive(true);
                yield return null;
                yield return StartCoroutine(ScaleSettingWindowRoutine(Vector3.zero, Vector3.one, timeScale));
                Time.timeScale = 0;
            }
            else if (!this._isShowing)
            {
                Time.timeScale = 1;
                yield return StartCoroutine(ScaleSettingWindowRoutine(Vector3.one, Vector3.zero, timeScale));
                this.gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator ScaleSettingWindowRoutine(Vector3 startScale, Vector3 desScale, float timeScale = 0.2f)
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
                //this.HighScoreWindow.transform.localScale = desScale;
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
}
