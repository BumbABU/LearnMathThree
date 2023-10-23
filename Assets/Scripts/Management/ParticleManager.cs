using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticlePlayer ClearFXPrefab;
    public ParticlePlayer BreakFXPrefab;
    public ParticlePlayer DoubleFXPrefab;
    public ParticlePlayer BombFXPrefab;

    public void ClearPieceFXAt(int x, int y, int z = 0)
    {
        if (ClearFXPrefab != null)
        {
            ParticlePlayer clearFX = Instantiate(ClearFXPrefab, new Vector3(x, y, z), Quaternion.identity) as ParticlePlayer;
            if (clearFX != null)
            {
                clearFX.Play();
            }
        }
    }

    public void BreakTileFXAt(int breakableValue, int x, int y, int z = 0)
    {
        ParticlePlayer breakFX = null;
        if (breakableValue > 1)
        {
            breakFX = Instantiate(DoubleFXPrefab, new Vector3(x, y, z), Quaternion.identity) as ParticlePlayer;
        }
        else
        {
            breakFX = Instantiate(BreakFXPrefab, new Vector3(x, y, z), Quaternion.identity) as ParticlePlayer;
        }
        if (breakFX != null)
        {
            breakFX.Play();
        }
    }

    public void BombFXAt(int x, int y, int z = 0)
    {
        if(this.BombFXPrefab != null)
        {
            ParticlePlayer bombFX = Instantiate (BombFXPrefab, new Vector3(x,y,z), Quaternion.identity) as ParticlePlayer;
            if (bombFX != null)
            {
                bombFX.Play();
            }
        }
    }
}
