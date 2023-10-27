using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardHighLighter : MonoBehaviour
{
    public Board Board;

    private void Awake()
    {
        Board = GetComponent<Board>();
    }
    public void HighLightMatches() // for test
    {
        for (int i = 0; i < Board.Width; i++)
        {
            for (int j = 0; j < Board.Height; j++)
            {
                HighLightMatchesAt(i, j);
            }
        }
    }

    public void HighLightMatchesAt(int x, int y)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardHighLighter");
            return;
        }
        this.HighLightTileOff(x, y);
        var combinedMatches = Board.BoardMatcher.FindMatchesAt(x, y);
        if (combinedMatches.Count > 0)
        {
            foreach (GamePiece gamePiece in combinedMatches)
            {
                HighLightTileOn(gamePiece.xIndex, gamePiece.yIndex, gamePiece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    public void HighLightTileOn(int x, int y, Color color)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardHighLighter");
            return;
        }
        if (Board.AllTiles[x, y].TileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = Board.AllTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
        }
    }

    public void HighLightTileOff(int x, int y)
    {
        if (Board.AllTiles[x, y].TileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = Board.AllTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        }
    }

    private void HighLightPieces(List<GamePiece> listgamePiece)
    {
        foreach (GamePiece piece in listgamePiece)
        {
            if (piece != null)
            {
                this.HighLightTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }
}
