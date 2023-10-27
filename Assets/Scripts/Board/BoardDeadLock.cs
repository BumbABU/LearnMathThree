using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardDeadLock : MonoBehaviour
{
    private List<GamePiece> GetRowOrColumnList(GamePiece[,] allPieces, int x, int y, int listLength = 3, bool checkRow = true)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);
        List<GamePiece> listGamePiece = new List<GamePiece>();
        for (int i = 0; i < listLength; i++)
        {
            if (checkRow)
            {
                if (x + i < width && y < height && allPieces[x + i, y] != null)
                {
                    listGamePiece.Add(allPieces[x + i, y]);
                }
            }
            else
            {
                if (x < width && y + i < height && allPieces[x, y + i] != null)
                {
                    listGamePiece.Add(allPieces[x, y + i]);
                }
            }
        }
        return listGamePiece;
    }
    private List<GamePiece> GetMinimumMatches(List<GamePiece> listGamePiece, int minForMatch = 2)
    {
        List<GamePiece> listMatches = new List<GamePiece>();

        var groups = listGamePiece.GroupBy(n => n.MatchValue);
        foreach (var group in groups)
        {
            if (group.Count() >= minForMatch && group.Key != MatchValue.None)
            {
                listMatches = group.ToList();
            }
        }
        return listMatches;
    }
    private List<GamePiece> GetNeighBors(GamePiece[,] allPieces, int x, int y)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);
        List<GamePiece> listNeighbor = new List<GamePiece>();
        Vector2[] searchDirection = new Vector2[4]
        {
           new Vector2(-1,0),
           new Vector2(1,0),
           new Vector2(0,-1),
           new Vector2(0,1)
        };
        foreach (Vector2 dir in searchDirection)
        {
            if (x + (int)dir.x >= 0 && x + (int)dir.x < width && y + (int)dir.y >= 0 && y + (int)dir.y < height)
            {
                if (allPieces[x + (int)dir.x, y + (int)dir.y] != null)
                {
                    if (!listNeighbor.Contains(allPieces[x + (int)dir.x, y + (int)dir.y]))
                    {
                        listNeighbor.Add(allPieces[x + (int)dir.x, y + (int)dir.y]);
                    }
                }
            }
        }
        return listNeighbor;
    }

    private bool HasMoveAt(GamePiece[,] allPieces, int x, int y, int listLength = 3, bool checkRow = true)
    {
        List<GamePiece> listPiece = this.GetRowOrColumnList(allPieces, x, y, listLength, checkRow);
        List<GamePiece> listMatchesPiece = this.GetMinimumMatches(listPiece, listLength - 1);
        GamePiece unmatchesPiece = null;
        if (listPiece != null && listMatchesPiece != null)
        {
            if (listPiece.Count == listLength && listMatchesPiece.Count == listLength - 1)
            {
                unmatchesPiece = listPiece.Except(listMatchesPiece).FirstOrDefault();
            }

            if (unmatchesPiece != null)
            {
                List<GamePiece> listNeighBor = this.GetNeighBors(allPieces, unmatchesPiece.xIndex, unmatchesPiece.yIndex);
                listNeighBor = listNeighBor.Except(listMatchesPiece).ToList();
                listNeighBor = listNeighBor.FindAll(n => n.MatchValue == listMatchesPiece[0].MatchValue);

                listMatchesPiece = listMatchesPiece.Union(listNeighBor).ToList();
            }

            if (listMatchesPiece.Count >= listLength)
            {
                Debug.Log("=====================AVAILABLE======================");
                return true;
            }
        }
        return false;
    }

    public bool IsDeadLocked(GamePiece[,] allPieces, int listLength = 3)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        bool isDeadLock = true;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (this.HasMoveAt(allPieces, i, j, listLength, true) || this.HasMoveAt(allPieces, i, j, listLength, false))
                {
                    isDeadLock = false;
                }
            }
        }
        if (isDeadLock)
        {
            Debug.Log("DEAD LOCK");
        }
        return isDeadLock;
    }
}
