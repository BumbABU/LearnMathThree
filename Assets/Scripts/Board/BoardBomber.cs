using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardBomber : MonoBehaviour
{
    public Board Board;

    private void Awake()
    {
        Board = GetComponent<Board>();
    }

    public void ProcessBombs(Tile clickedTile, Tile targetTile, List<GamePiece> clickedPieceMatches, List<GamePiece> targetPieceMatches)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardBomber");
            return;
        }
        Vector2 swipeDirection = new Vector2(targetTile.xIndex - clickedTile.xIndex, targetTile.yIndex - clickedTile.yIndex);
        Board.ClickedTileBomb = this.DropBomb(clickedTile.xIndex, clickedTile.yIndex, swipeDirection, clickedPieceMatches);
        Board.TargetTileBomb = this.DropBomb(targetTile.xIndex, targetTile.yIndex, swipeDirection, targetPieceMatches);

        /*if (this._clickedTileBomb != null && targetPiece != null)
        {
            if (!this.IsColorBomb(this._clickedTileBomb))
            {
                this._clickedTileBomb.ChangeColor(targetPiece);
            }
        }

        if (this._targetTileBomb != null && clickedPiece != null)
        {
            if (!this.IsColorBomb(this._targetTileBomb))
            {
                this._targetTileBomb.ChangeColor(clickedPiece);
            }
        }*/ // oldversion (change Color)
    }

    public List<GamePiece> ProcessColorBomb(GamePiece clickedPiece, GamePiece targetPiece, bool clearNonBlockers = false)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardBomber");
            return null;
        }
        List<GamePiece> colorMatches = new List<GamePiece>();
        GamePiece colorBombPiece = null;
        GamePiece otherPiece = null;
        if (Board.BoardQuery.IsColorBomb(clickedPiece) && !Board.BoardQuery.IsColorBomb(targetPiece))
        {
            colorBombPiece = clickedPiece;
            otherPiece = targetPiece;
            /*            clickedPiece.MatchValue = targetPiece.MatchValue;
                        colorMatches = this.FindAllPieceByMatchValue(clickedPiece.MatchValue);*/
        }
        else if (!Board.BoardQuery.IsColorBomb(clickedPiece) && Board.BoardQuery.IsColorBomb(targetPiece))
        {
            colorBombPiece = targetPiece;
            otherPiece = clickedPiece;
            /*            targetPiece.MatchValue = clickedPiece.MatchValue;
                        colorMatches = this.FindAllPieceByMatchValue(targetPiece.MatchValue);*/
        }
        else if (Board.BoardQuery.IsColorBomb(clickedPiece) && Board.BoardQuery.IsColorBomb(targetPiece))
        {
            foreach (GamePiece gamePiece in Board.AllGamePieces)
            {
                if (!colorMatches.Contains(gamePiece))
                {
                    colorMatches.Add(gamePiece);
                }
            }
        }
        if (colorBombPiece != null)
        {
            colorBombPiece.MatchValue = otherPiece.MatchValue;
            colorMatches = Board.BoardQuery.FindAllPieceByMatchValue(colorBombPiece.MatchValue);
        }
        if (!clearNonBlockers)
        {
            List<GamePiece> collectedAtbottom = Board.BoardQuery.FindAllCollectibles(true);
            if (collectedAtbottom.Contains(otherPiece))
            {
                return new List<GamePiece>();
            }
            else
            {
                foreach (GamePiece piece in collectedAtbottom)
                {
                    if (colorMatches.Contains(piece))
                    {
                        colorMatches.Remove(piece);
                    }
                }
            }
        }

        return colorMatches;
    }

    public Bomb DropBomb(int x, int y, Vector2 swapDirection, List<GamePiece> listGamePiece)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardBomber");
            return null;
        }
        Bomb bomb = null;
        MatchValue matchValue = MatchValue.None;
        if (listGamePiece != null)
        {
            matchValue = Board.BoardQuery.FindMatchValue(listGamePiece);
        }
        if (listGamePiece.Count >= 5 && matchValue != MatchValue.None)
        {
            if (Board.BoardQuery.IsCornerMatch(listGamePiece))
            {
                GamePiece adjacentBomb = Board.BoardQuery.FindGamePieceByMatchValue(Board.ListAdjacentBombPrefab, matchValue);
                if (adjacentBomb != null)
                {
                    bomb = Board.BoardFiller.MakeBomb(adjacentBomb, x, y);
                }
            }
            else
            {
                if (Board.ColorBombPrefab != null)
                {
                    bomb = Board.BoardFiller.MakeBomb(Board.ColorBombPrefab, x, y);
                }
            }

        }
        else if (listGamePiece.Count >= 4)
        {
            if (swapDirection.x != 0)
            {
                GamePiece rowBomb = Board.BoardQuery.FindGamePieceByMatchValue(Board.ListRowBombPrefab, matchValue);
                if (rowBomb != null)
                {
                    bomb = Board.BoardFiller.MakeBomb(rowBomb, x, y);
                }
            }
            else
            {
                GamePiece comlumnBomb = Board.BoardQuery.FindGamePieceByMatchValue(Board.ListColumnBombPrefab, matchValue);
                if (comlumnBomb != null)
                {
                    bomb = Board.BoardFiller.MakeBomb(comlumnBomb, x, y);
                }
            }
        }
        return bomb;
    }

    public void ActivateBombs() // for Board
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardBomber");
            return;
        }
        if (Board.ClickedTileBomb != null)
        {
            this.ActivateBomb(Board.ClickedTileBomb);
            Board.ClickedTileBomb = null;
        }

        if (Board.TargetTileBomb != null)
        {
            this.ActivateBomb(Board.TargetTileBomb);
            Board.TargetTileBomb = null;
        }
    }

    public void ActivateBomb(Bomb bomb)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardBomber");
            return;
        }
        int x = (int)bomb.transform.position.x;
        int y = (int)bomb.transform.position.y;
        if (Board.BoardQuery.IsWithinBounds(x, y))
        {
            Board.AllGamePieces[x, y] = bomb;
        }
    }

}
