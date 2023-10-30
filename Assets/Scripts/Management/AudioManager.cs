using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    /*private static readonly string FirstPlay = "FirstPlay";
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
        PlayerPrefs.SetFloat(soundEffectsPref, soundEffectSlider.value);
        PlayerPrefs.SetFloat(backgroundPref, backgroundSlider.value);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Debug.Log("InFocus");
        }
        else
        {
            Debug.Log("OutFocus");
        }
    }

    public void UpdateAudio()
    {
        BackgroundAudio.volume = this.backgroundSlider.value;
        this.SaveAudioSetting();
        Debug.Log("RunSlider");
    }*/
    public AudioSource AttachBGMSource;
    public AudioSource AttachSESource;
    private static readonly string BGMPrefVol = "VolumeBGMPref";
    private static readonly string SEPrefVol = "VolumeSEPref";
    private Dictionary<string, AudioClip> seDics = new Dictionary<string, AudioClip>();
    public List<AudioClip> SEClips;
    private float BGMVoldefault = 0.125f;
    private float SEVoldefault = 0.7f;

    public override void Awake()
    {
        base.Awake();
        foreach (AudioClip seClip in SEClips)
        {
            if(seClip)
            {
                this.seDics[seClip.name] = seClip;
            }
        }
    }

    private void Start()
    {
        this.SetBGMVol();
        this.SetSEVol();
    }

    private void SetBGMVol()
    {
        if(!PlayerPrefs.HasKey(BGMPrefVol))
        {
            this.AttachBGMSource.volume = BGMVoldefault;
            PlayerPrefs.SetFloat(BGMPrefVol, BGMVoldefault);
        }
        else
        {
            this.AttachBGMSource.volume = PlayerPrefs.GetFloat(BGMPrefVol);
        }

    }

    private void SetSEVol()
    {
        if (!PlayerPrefs.HasKey(SEPrefVol))
        {
            this.AttachSESource.volume = SEVoldefault;
            PlayerPrefs.SetFloat(SEPrefVol, SEVoldefault);
        }
        else
        {
            this.AttachSESource.volume = PlayerPrefs.GetFloat(SEPrefVol);
        }
    }

    public void PlayBGM()
    {
        if (this.AttachBGMSource != null)
        {
            if (!this.AttachBGMSource.isPlaying)
            {
                this.AttachBGMSource.loop = true;
                this.AttachBGMSource.Play();
            }
        }
    }

    public void StopBGM()
    {
        if (this.AttachBGMSource != null)
        {
            if (this.AttachBGMSource.isPlaying)
            {
                this.AttachBGMSource.Stop();
            }
        }
    }
    public void PlaySE(string name)
    {
        if (this.AttachSESource != null)
        {
            if(this.seDics.ContainsKey(name))
            {
                this.AttachSESource.PlayOneShot(seDics[name]);
            }
        }
    }

    public void ChangeBGMVolume(float volume)
    {
        this.AttachBGMSource.volume = volume;
        PlayerPrefs.SetFloat(BGMPrefVol, volume);
    }

    public void ChangeSEVolume(float volume)
    {
        this.AttachSESource.volume = volume;
        PlayerPrefs.SetFloat(SEPrefVol, volume);
    }
}
