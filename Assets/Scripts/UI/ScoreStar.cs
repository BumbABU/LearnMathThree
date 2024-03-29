using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreStar : MonoBehaviour
{
    public Image Star;
    public ParticlePlayer StarFX;
    public float Delay = 0.5f;
    public AudioClip StarSound;
    public bool Activated = false;

    public void Start()
    {
        this.SetActive(false);
    }

    private void SetActive(bool state)
    {
        if (this.Star != null)
        {
            this.Star.gameObject.SetActive(state);
        }
    }

    public void Activate()
    {
        if (this.Activated) return;
        StartCoroutine(ActivateRoutine());

    }

    private IEnumerator ActivateRoutine()
    {
        this.Activated = true;
        if(this.StarFX != null)
        {
            StarFX.Play();
        }
        if(AudioManager.Instance)
        {
            AudioManager.Instance.PlaySE(AUDIO.FX_6);
        }
        yield return new WaitForSeconds(Delay);
        this.SetActive(true);
    }
}
