using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectXformMover : MonoBehaviour
{
    public Vector3 StartPosition;
    public Vector3 OnscreenPostion;
    public Vector3 EndPosition;

    public float TimeToMove = 1f;

    private RectTransform _rectXform;
    private bool _isMoving = false;

    public void Awake()
    {
        this._rectXform = GetComponent<RectTransform>();
    }

    private void Move(Vector3 startPos, Vector3 endPos, float timeToMove)
    {
        if(!this._isMoving)
        {
            StartCoroutine(MoveRoutine(startPos, endPos, timeToMove));
        }
    }

    private IEnumerator MoveRoutine(Vector3 startPos, Vector3 endPos, float timeToMove)
    {
        if(this._rectXform != null)
        {
            this._rectXform.anchoredPosition = startPos;
        }
        this._isMoving = true;
        float elapsedTime = 0;
        bool reachDestination = false;
        while(!reachDestination)
        {
            if(Vector3.Distance(this._rectXform.anchoredPosition, endPos) < 0.01)
            {
                reachDestination = true;
                break;
            }
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime/timeToMove);
            MakeSmoothNum.SmootherStep(t);
            if(this._rectXform != null)
            {
                this._rectXform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            }
            yield return null;
        }
        this._isMoving = false;
    }

    public void MoveOn()
    {
        this.Move(this.StartPosition, this.OnscreenPostion, this.TimeToMove);
    }

    public void MoveOff()
    {
        this.Move(this.OnscreenPostion, this.EndPosition, this.TimeToMove);
    }
}
