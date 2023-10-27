using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardCollapser : MonoBehaviour
{
    public Board Board;
    private void Awake()
    {
        Board = GetComponent<Board>();
    }

    public List<GamePiece> CollapseColumn(int column, float collapseTime = 0.1f) // input (number of column) => out put (game piece to collape)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardCollapser");
            return null;
        }
        List<GamePiece> movingPieces = new List<GamePiece>();

        for (int i = 0; i < Board.Height - 1; i++)
        {
            if (Board.AllGamePieces[column, i] == null && Board.AllTiles[column, i].TileType != TileType.Obstacle)
            {
                for (int j = i + 1; j < Board.Height; j++)
                {
                    if (Board.AllGamePieces[column, j] != null)
                    {
                        Board.AllGamePieces[column, j].Move(column, i, collapseTime * (j - i));
                        Board.AllGamePieces[column, i] = Board.AllGamePieces[column, j];
                        Board.AllGamePieces[column, i].SetCoord(column, i);
                        if (!movingPieces.Contains(Board.AllGamePieces[column, i]))
                        {
                            movingPieces.Add(Board.AllGamePieces[column, i]);
                        }
                        Board.AllGamePieces[column, j] = null;
                        break;
                    }
                }
            }
        }
        return movingPieces;
    }

    public List<GamePiece> CollapseColumn(List<int> columnsToCollapse)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardCollapser");
            return null;
        }
        List<GamePiece> movingPieces = new List<GamePiece>();
        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(this.CollapseColumn(column)).ToList();
        }
        return movingPieces;
    }
}
