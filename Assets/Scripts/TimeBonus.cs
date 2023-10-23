using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GamePiece))]
public class TimeBonus : MonoBehaviour
{
    [Range(0, 5)]
    public int BonusValue = 5;

    [Range(0f, 1f)]
    public float ChanceBonus = 0.1f;

    public GameObject BonusGlow;
    public Material[] BonusMaterial;
    public GameObject RingGlow;

    public void Start()
    {
        float random = Random.Range(0f, 1f);
        if(random > this.ChanceBonus)
        {
            this.BonusValue = 0;
        }
        if(GameManager.Instance)
        {
            if(GameManager.Instance.LevelGoal.LevelCounter != LevelCounter.Timer)
            {
                this.BonusValue = 0;
            }
        }
        this.SetActive(this.BonusValue != 0);
        if(this.BonusValue != 0)
        {
            this.SetupMaterial(this.BonusValue - 1, this.BonusGlow);
        }
    }

    private void SetActive(bool state)
    {
        if(this.BonusGlow != null)
        {
            this.BonusGlow.SetActive(state);
        }
        if(this.RingGlow != null)
        {
            this.RingGlow.SetActive(state);
        }
    }

    private void SetupMaterial(int value, GameObject bonusGlow)
    {
        int clampedValue = Mathf.Clamp(value, 0, this.BonusMaterial.Length -1);
        if (this.BonusMaterial[clampedValue] != null)
        {
            if(bonusGlow != null)
            {
               ParticleSystemRenderer bonusGlowRenderer = bonusGlow.GetComponent<ParticleSystemRenderer>();
               if(bonusGlowRenderer != null)
                {
                    bonusGlowRenderer.material = this.BonusMaterial[clampedValue];
                }
            }
        }
    }
}
