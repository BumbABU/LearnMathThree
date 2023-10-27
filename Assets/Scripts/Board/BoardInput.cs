using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardInput : MonoBehaviour
{
    public Board Board;

    private void Awake()
    {
        Board = GetComponent<Board>();
    }

    public void ClickTile(Tile tile)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardInput");
            return;
        }
        if (Board.ClickedTile == null)
        {
            Board.ClickedTile = tile;
        }
    }

    public void DragToTile(Tile tile)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardInput");
            return;
        }
        if (Board.ClickedTile != null && Board.BoardQuery.IsNextTo(tile, Board.ClickedTile))
        {
            Board.TargetTile = tile;
        }
        else
        {
            Board.TargetTile = null;
        }
    }

    public void ReleaseTile()
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardInput");
            return;
        }
        if (Board.ClickedTile != null && Board.TargetTile != null)
        {
            Board.SwitchTile(Board.ClickedTile, Board.TargetTile);
        }
        Board.ClickedTile = null;
        Board.TargetTile = null;
    }

}
