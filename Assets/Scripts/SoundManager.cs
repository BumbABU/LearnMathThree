using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioClip[] MusicClips;
    public AudioClip[] WinClips;
    public AudioClip[] LoseClips;
    public AudioClip[] BonusClips;

    [Range(0f, 1f)]
    public float MusicVolume;

    [Range(0f, 1f)]
    public float FxVolume;
    public float LowPitch = 0.95f;
    public float HighPitch = 1.05f;
    public void Start()
    {
    }

    public AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1f , bool randomPitch = true)
    {
        if(clip != null)
        {
            GameObject go = new GameObject($"SoundFX{clip.name}");
            go.transform.position = position;

            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            if(randomPitch)
            {
                float ramdomPitch = Random.Range(this.LowPitch, this.HighPitch);
                source.pitch = ramdomPitch;
            }
            source.Play();
            Destroy(go, clip.length);
            return source;
        }
        return null;
    }

    public AudioSource PlayRandomClip(AudioClip[] clips, Vector3 position, float volume = 1f)
    {
        if(clips != null)
        {
            if(clips.Length > 0)
            {
                int randomIndex  = Random.Range(0, clips.Length);
                if (clips[randomIndex] != null)
                {
                    AudioSource source = this.PlayClipAtPoint(clips[randomIndex], position, volume);
                    return source;
                }
            }
        }
        return null;
    }

    public void PlayRandomMusic()
    {
        this.PlayRandomClip(this.MusicClips, Vector3.zero, MusicVolume);
    }

    public void PlayRandomWinSound()
    {
        this.PlayRandomClip(this.WinClips, Vector3.zero, MusicVolume);
    }

    public void PlayRandomLoseSound()
    {
        this.PlayRandomClip(this.LoseClips, Vector3.zero, MusicVolume);
    }

    public void PlayRandomBonusSound()
    {
        this.PlayRandomClip(this.BonusClips, Vector3.zero, MusicVolume);
    }
}
