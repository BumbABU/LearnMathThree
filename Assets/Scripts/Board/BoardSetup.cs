using System.Collections.Generic;
using UnityEngine;
using static Board;

[RequireComponent(typeof(Board))]
public class BoardSetup : MonoBehaviour
{
    public Board Board;
    private void Awake()
    {
        Board = GetComponent<Board>();
    }

    public void SetUpBoard()
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardSetup");
            return;
        }
        this.SetupCamera();
        this.SetupTiles();
        this.SetupGamePiece();
        List<GamePiece> listStartingCollectible = Board.BoardQuery.FindAllCollectibles();
        Board.CollectibleCount = listStartingCollectible.Count;
        Board.BoardFiller.FillBoard(Board.FillYOffset, Board.FillMoveTime);
    }

    private void SetupCamera()
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardSetup");
            return;
        }
        Camera.main.transform.position = new Vector3((Board.Width - 1) / 2, (Board.Height - 1) / 2, -10);
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float horizontalSize = ((float)Board.Width / 2 + (float)Board.BorderSize) / aspectRatio;
        float verticalSize = (float)Board.Height / 2 + (float)Board.BorderSize;
        Camera.main.orthographicSize = verticalSize > horizontalSize ? verticalSize : horizontalSize;
    }

    private void SetupTiles()
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardSetup");
            return;
        }
        foreach (StartingObject startingTile in Board.ListStartingTiles)
        {
            if (startingTile != null)
            {
                Board.BoardFiller.MakeTile(startingTile.Prefab.GetComponent<Tile>(), startingTile.x, startingTile.y, startingTile.z);
            }
        }

        for (int i = 0; i < Board.Width; i++)
        {
            for (int j = 0; j < Board.Height; j++)
            {
                if (Board.AllTiles[i, j] == null)
                {
                    Board.BoardFiller.MakeTile(Board.NormalTile, i, j);
                }
            }
        }
    }

    private void SetupGamePiece() // starting gamepice only 
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardSetup");
            return;
        }
        foreach (StartingObject sGamePiece in Board.ListStartingGamePiece)
        {
            if (sGamePiece != null)
            {
                GamePiece gamePiece = Instantiate(sGamePiece.Prefab.GetComponent<GamePiece>(), new Vector3(sGamePiece.x, sGamePiece.y, 0), Quaternion.identity) as GamePiece;
                Board.BoardFiller.MakeGamePiece(gamePiece, sGamePiece.x, sGamePiece.y, Board.FillYOffset, Board.FillMoveTime);
            }
        }
    }
}
