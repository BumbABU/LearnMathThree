using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardTiles : MonoBehaviour
{
    public Board Board;

    private void Awake()
    {
        Board = GetComponent<Board>();
    }

    public void BreakTileAt(int x, int y)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardTile");
            return;
        }
        Tile tiletoBreak = Board.AllTiles[x, y];
        if (tiletoBreak != null && tiletoBreak.TileType == TileType.Breakable)
        {
            if (Board.ParticleManager != null)
            {
                Board.ParticleManager.BreakTileFXAt(tiletoBreak.BreakableValue, x, y, 0);
            }
            tiletoBreak.BreakTile();
        }
    }

    public void BreakTileAt(List<GamePiece> listgamePiece)
    {
        foreach (GamePiece piece in listgamePiece)
        {
            if (piece != null)
            {
                this.BreakTileAt(piece.xIndex, piece.yIndex);
            }
        }
    }
}
