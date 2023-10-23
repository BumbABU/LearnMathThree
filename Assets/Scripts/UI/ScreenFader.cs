using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour
{
    public float SolidAlpha = 1f;
    public float ClearAlpha = 0f;
    public float Delay = 0f;
    public float TimeToFade = 1f;

    private MaskableGraphic _graphic;
    void Start()
    {
        this._graphic = GetComponent<MaskableGraphic>();
        //this.FadeOff();
    }

    private IEnumerator FadeRoutine(float alpha)
    {
        yield return new WaitForSeconds(this.Delay);
        this._graphic.CrossFadeAlpha(alpha, this.TimeToFade, true);
    }

    public void FadeOn()
    {
        StartCoroutine(FadeRoutine(this.SolidAlpha));
    }

    public void FadeOff()
    {
        StartCoroutine (FadeRoutine(this.ClearAlpha));
    }
}
