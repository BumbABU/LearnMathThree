using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardClearer : MonoBehaviour
{
    public Board Board;

    private void Awake()
    {
        Board = GetComponent<Board>();
    }

    public void ClearBoard()
    {
        if (Board == null) return;
        for (int i = 0; i < Board.Width; i++)
        {
            for (int j = 0; j < Board.Height; j++)
            {
                this.ClearPieceAt(i, j);
                if (Board.ParticleManager != null)
                {
                    Board.ParticleManager.ClearPieceFXAt(i, j);
                }
            }
        }
    }

    public void ClearPieceAt(int x, int y)
    {
        GamePiece gamePieceToClear = Board.AllGamePieces[x, y];
        if (gamePieceToClear != null)
        {
            Board.AllGamePieces[x, y] = null;
            Destroy(gamePieceToClear.gameObject);
        }
        // HighLightTileOff(x, y);
    }

    public void ClearPieceAt(List<GamePiece> listgamePiece, List<GamePiece> listBomb)
    {
        foreach (GamePiece gamePiece in listgamePiece)
        {
            if (gamePiece != null)
            {
                int bonus = 0;
                this.ClearPieceAt(gamePiece.xIndex, gamePiece.yIndex);
                if (listgamePiece.Count >= 4)
                {
                    bonus = 10;
                }
                if (GameManager.Instance)
                {
                    GameManager.Instance.ScorePoints(gamePiece, Board.ScoreMultiplier, bonus);
                    TimeBonus timeBonus = gamePiece.GetComponent<TimeBonus>();
                    if (timeBonus != null)
                    {
                        GameManager.Instance.AddTime(timeBonus.BonusValue);
                        Debug.Log("Add time bonus: " + timeBonus.BonusValue);
                    }
                    GameManager.Instance.UpdateCollectionsGoals(gamePiece);
                }
                if (Board.ParticleManager != null)
                {
                    if (listBomb.Contains(gamePiece))
                    {
                        Board.ParticleManager.BombFXAt(gamePiece.xIndex, gamePiece.yIndex);
                    }
                    else
                    {
                        Board.ParticleManager.ClearPieceFXAt(gamePiece.xIndex, gamePiece.yIndex);
                    }
                }
            }
        }
    }
}
