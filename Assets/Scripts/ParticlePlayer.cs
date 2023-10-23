using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    public ParticleSystem[] AllParticle;
    public float LifeTime = 1f;
    public bool DestroyImmediately = true;
    private void Start()
    {
        this.AllParticle = GetComponentsInChildren<ParticleSystem>();
        if(this.DestroyImmediately)
        {
            Destroy(gameObject, LifeTime);
        }
    }

    // Update is called once per frame
    public void Play()
    {
        foreach (ParticleSystem p in AllParticle)
        {
            if (p != null)
            {
                p.Stop();
                p.Play();
            }
        }
        Destroy(this.gameObject, this.LifeTime);
    }
}
