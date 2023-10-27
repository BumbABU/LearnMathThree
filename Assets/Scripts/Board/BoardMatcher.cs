using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardMatcher : MonoBehaviour
{
    public Board Board;

    private void Awake()
    {
        Board = GetComponent<Board>();
    }

    public List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardQuery");
            return null;
        }
        List<GamePiece> matches = new List<GamePiece>();
        GamePiece startPiece = null;
        if (Board.BoardQuery.IsWithinBounds(startX, startY))
        {
            startPiece = Board.AllGamePieces[startX, startY];
        }
        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }
        int nextX;
        int nextY;

        int maxValue = (Board.Width > Board.Height) ? Board.Width : Board.Height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!Board.BoardQuery.IsWithinBounds(nextX, nextY))
            {
                break;
            }
            GamePiece nextPiece = Board.AllGamePieces[nextX, nextY];

            if (nextPiece == null)
            {
                break;
            }
            else
            {
                if (nextPiece.MatchValue == startPiece.MatchValue && !matches.Contains(nextPiece) && nextPiece.MatchValue != MatchValue.None)
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }

        if (matches.Count >= minLength)
        {
            return matches;
        }
        return null;
    }

    private List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = this.FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<GamePiece> downwardMatches = this.FindMatches(startX, startY, new Vector2(0, -1), 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }
        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        //option1 default

        /*foreach (GamePiece gamePiece in downwardMatches)
        {
            if (!upwardMatches.Contains(gamePiece))
            {
                upwardMatches.Add(gamePiece);
            }
        }

        return (upwardMatches.Count >= minLength) ? upwardMatches : null;*/

        //option2 Linq
        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();
        return (combinedMatches.Count >= minLength) ? combinedMatches : null;
    }

    private List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> rightMatches = this.FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<GamePiece> leftMatches = this.FindMatches(startX, startY, new Vector2(-1, 0), 2);

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();
        return (combinedMatches.Count >= minLength) ? combinedMatches : null;
    }

    public List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<GamePiece> horizontalMatches = this.FindHorizontalMatches(x, y, minLength);
        List<GamePiece> verticalMatches = this.FindVerticalMatches(x, y, minLength);
        if (horizontalMatches == null)
        {
            horizontalMatches = new List<GamePiece>();
        }
        if (verticalMatches == null)
        {
            verticalMatches = new List<GamePiece>();
        }

        var combinedMatches = verticalMatches.Union(horizontalMatches).ToList();
        return combinedMatches;
    }

    public List<GamePiece> FindMatchesAt(List<GamePiece> listgamePiece, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        foreach (GamePiece piece in listgamePiece)
        {
            matches = matches.Union(FindMatchesAt(piece.xIndex, piece.yIndex, minLength)).ToList();
        }
        return matches;
    }

    public List<GamePiece> FindAllMatches()
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardMatcher");
            return null;
        }
        List<GamePiece> combinedMatches = new List<GamePiece>();
        for (int i = 0; i < Board.Width; i++)
        {
            for (int j = 0; j < Board.Height; j++)
            {
                List<GamePiece> matches = this.FindMatchesAt(i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }
        return combinedMatches;
    }
}
