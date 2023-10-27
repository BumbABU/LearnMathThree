using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardShuffler : MonoBehaviour
{
    public List<GamePiece> RemoveNormalPieces(GamePiece[,] allPieces)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);
        List<GamePiece> listNormalPiece = new List<GamePiece>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allPieces[i, j] != null)
                {
                    Bomb bomb = allPieces[i, j].GetComponent<Bomb>();
                    Collectible collectible = allPieces[i, j].GetComponent<Collectible>();
                    if (bomb == null && collectible == null)
                    {
                        listNormalPiece.Add(allPieces[i, j]);
                        allPieces[i, j] = null;
                    }
                }
            }
        }
        return listNormalPiece;
    }

    public void ShuffleList(List<GamePiece> listPieceToShuffle)
    {
        int maxCount = listPieceToShuffle.Count;
        for (int i = 0; i < maxCount - 1; i++)
        {
            int r = Random.Range(i, maxCount);
            if (r == i) continue;
            GamePiece temp = listPieceToShuffle[i];
            listPieceToShuffle[i] = listPieceToShuffle[r];
            listPieceToShuffle[r] = temp;
        }
    }

    public void MovePieces(GamePiece[,] allPieces, float swapTime = 0.5f)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allPieces[i, j] != null)
                {
                    allPieces[i, j].Move(i, j, swapTime);
                }
            }
        }
    }

    public IEnumerator ShuffleBoardRoutine(Board board)
    {
        if(board != null)
        {
            List<GamePiece> listAllGamePiece = new List<GamePiece>();
            foreach (GamePiece piece in board.AllGamePieces)
            {
                listAllGamePiece.Add(piece);
            }

            while (!board.BoardQuery.IsCollapsed(listAllGamePiece))
            {
                yield return null;
            }

            List<GamePiece> normalPieces = this.RemoveNormalPieces(board.AllGamePieces);
            this.ShuffleList(normalPieces);
            board.BoardFiller.FillBoardFromList(normalPieces);
            this.MovePieces(board.AllGamePieces, board.SwapTime);

            List<GamePiece> matches = board.BoardMatcher.FindAllMatches();
            StartCoroutine(board.ClearAndRefillBoardRoutine(matches));
        }
    }
}
