using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string soundEffectsPref = "SoundEffectsPref";
    private static readonly string backgroundPref = "BackgroundPref";
    private int firstPlayInt;
    public Slider backgroundSlider, soundEffectSlider;
    private float backgroundFloat, soundEffectFloat;

    public AudioSource BackgroundAudio;
    public AudioSource SoundEffectAudio;

    private void Start()
    {
        firstPlayInt = PlayerPrefs.GetInt(FirstPlay);
        if (firstPlayInt == 0)
        {
            this.soundEffectFloat = 0.75f;
            this.backgroundFloat = 0.125f;
          //  this.soundEffectSlider.value = this.soundEffectFloat;
            this.backgroundSlider.value = this.backgroundFloat;
            PlayerPrefs.SetFloat(soundEffectsPref, soundEffectFloat);
            PlayerPrefs.SetFloat(backgroundPref, backgroundFloat);
            PlayerPrefs.SetInt("FirstPlay", 1);
            Debug.Log("Run1");
        }
        else
        {
            this.BackgroundAudio.volume = this.backgroundFloat;
            this.soundEffectFloat = PlayerPrefs.GetFloat(soundEffectsPref);
            this.backgroundFloat = PlayerPrefs.GetFloat(backgroundPref);
            Debug.Log(backgroundFloat);
          //  this.soundEffectSlider.value = this.soundEffectFloat;
            this.backgroundSlider.value = this.backgroundFloat;
        }
    }

    public void SaveAudioSetting()
    {
       // PlayerPrefs.SetFloat(soundEffectsPref, soundEffectSlider.value);
        PlayerPrefs.SetFloat (backgroundPref, backgroundSlider.value);
    }

    private void OnApplicationFocus(bool focus)
    {
        if(focus)
        {
            Debug.Log("InFocus");
        }
        else
        {
            Debug.Log("OutFocus");
        }
    }

    public void UpdateAudio ()
    {
        BackgroundAudio.volume = this.backgroundSlider.value;
        this.SaveAudioSetting();
        Debug.Log("RunSlider");
    }
}
