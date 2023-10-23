using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InterpType
{
    Linear,
    EaseOut,
    EaseIn,
    SmoothStep,
    SmootherStep
}

public enum MatchValue
{
    Blue,
    Brown,
    Green,
    Pink,
    Purple,
    While,
    Yellow,
    Red,
    Black,
    Orange,
    Teal,
    None
}
[RequireComponent(typeof(SpriteRenderer))]
public class GamePiece : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    public InterpType InterPolationType = InterpType.SmootherStep;
    public MatchValue MatchValue;

    private bool _isMoving = false;
    private Board _board;

    public int ScoreValue = 20;
    public AudioClip ClearSound;
    public void Init(Board board)
    {
        this._board = board;
    }

    public void SetCoord(int x, int y)
    {
        this.xIndex = x;
        this.yIndex = y;
    }

    public void Move(int destX, int destY, float timeMove)
    {
        if (!_isMoving)
        {
            StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), timeMove));
        }
    }

    private IEnumerator MoveRoutine(Vector3 destination, float timeMove)
    {
        Vector3 startPos = transform.position;
        bool reachedDestination = false;
        float elapsedTime = 0f;
        this._isMoving = true;
        while (!reachedDestination)
        {
            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                reachedDestination = true;
                if (this._board != null)
                {
                    _board.PlaceGamePiece(this, (int)destination.x, (int)destination.y);
                }
                break;
            }
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / timeMove, 0f, 1f);
            switch (this.InterPolationType)
            {
                case InterpType.Linear:
                    break;
                case InterpType.EaseOut:
                    MakeSmoothNum.EaseOut(t);
                    break;
                case InterpType.EaseIn:
                    MakeSmoothNum.EaseIn(t);
                    break;
                case InterpType.SmoothStep:
                    MakeSmoothNum.SmoothStep(t);
                    break;
                case InterpType.SmootherStep:
                    MakeSmoothNum.SmootherStep(t);
                    break;
            }
            transform.position = Vector3.Lerp(startPos, destination, t);
            yield return null;
        }
        this._isMoving = false;

    }

    public void ChangeColor(GamePiece piecetoMatch)
    {
        SpriteRenderer renderertoChange = GetComponent<SpriteRenderer>();
        renderertoChange.color = Color.clear;
        if (piecetoMatch != null)
        {
            SpriteRenderer rendererToMatch = piecetoMatch.GetComponent<SpriteRenderer>();
            if (rendererToMatch != null && renderertoChange != null)
            {
                renderertoChange.color = rendererToMatch.color;
            }
            MatchValue = piecetoMatch.MatchValue;
        }
    }

}
