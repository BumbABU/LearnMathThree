using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI TimeLeftText;
    public Image ClockImage;

    private int _maxTime = 60;
    public bool Pause = false;

    public int FlashTimeLimit = 10;
    public AudioClip FlashBeep;
    public float FlashInterval = 1f;
    public Color FlashColor = Color.red;

    private IEnumerator _flashRoutine;
    public void InitTimer(int maxTime = 60)
    {
        this._maxTime = maxTime;
        if (this.ClockImage != null)
        {
            this.ClockImage.type = Image.Type.Filled; // just Double check setup with unity editor
            this.ClockImage.fillMethod = Image.FillMethod.Radial360; // just Double check setup with unity editor
            this.ClockImage.fillOrigin = (int)Image.Origin360.Top; // just Double check setup with unity editor , why add (int) because Origin360.Top is Enum 
            // Different way : this.ClockImage.fillOrigin = 2;
        }

        if (this.TimeLeftText != null)
        {
            this.TimeLeftText.text = maxTime.ToString();
        }
    }

    public void UpdateTimer(int currentTime)
    {
        if (this.Pause)
        {
            return;
        }
        if (this.ClockImage != null)
        {
            this.ClockImage.fillAmount = (float)currentTime / (float)this._maxTime;
            if (currentTime <= this.FlashTimeLimit)
            {
                this._flashRoutine = FlashRoutine(ClockImage, FlashColor, this.FlashInterval);
                StartCoroutine(this._flashRoutine);
                if (SoundManager.Instance && this.FlashBeep != null)
                {
                    SoundManager.Instance.PlayClipAtPoint(this.FlashBeep, Vector3.zero, SoundManager.Instance.FxVolume, false);
                }
            }
        }
        if (this.TimeLeftText != null)
        {
            this.TimeLeftText.text = currentTime.ToString();
        }
    }

    IEnumerator FlashRoutine(Image image, Color targetColor, float interval)
    {
        if (image != null)
        {
            Color originalColor = image.color;
            image.CrossFadeColor(targetColor, interval * 0.3f, true, true);
            yield return new WaitForSeconds(this.FlashInterval / 2);
            image.CrossFadeColor(originalColor, interval * 0.3f, true, true);
            yield return new WaitForSeconds(this.FlashInterval / 2);
        }
    }

    public void FadeOff()
    {
        if (this._flashRoutine != null)
        {
            StopCoroutine(this._flashRoutine);
        }
        ScreenFader[] screenFaders = GetComponentsInChildren<ScreenFader>();
        foreach (ScreenFader fader in screenFaders)
        {
            fader.FadeOff();
        }
    }
}
