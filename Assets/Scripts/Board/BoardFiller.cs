using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardFiller : MonoBehaviour
{
    public Board Board;
    private void Awake()
    {
        Board = GetComponent<Board>();
    }

    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardFiller");
            return;
        }
        if (gamePiece == null)
        {
            Debug.LogWarning("BOARD: Invalid GamePiece");
            return;
        }
        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;
        if (Board.BoardQuery.IsWithinBounds(x, y))
        {
            Board.AllGamePieces[x, y] = gamePiece;
        }
        gamePiece.SetCoord(x, y);
    }

    public void FillBoardFromList(List<GamePiece> listAllPiece)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardFiller");
            return;
        }
        Queue<GamePiece> unusedGamePiece = new Queue<GamePiece>(listAllPiece);

        int maxInterations = 100;
        int interations = 0;

        for (int i = 0; i < Board.Width; i++)
        {
            for (int j = 0; j < Board.Height; j++)
            {
                if (Board.AllGamePieces[i, j] == null && Board.AllTiles[i, j].TileType != TileType.Obstacle)
                {
                    Board.AllGamePieces[i, j] = unusedGamePiece.Dequeue();
                    interations = 0;
                    while (Board.BoardQuery.HasMatchOnFill(j, j))
                    {
                        interations++;
                        unusedGamePiece.Enqueue(Board.AllGamePieces[i, j]);
                        Board.AllGamePieces[i, j] = unusedGamePiece.Dequeue();
                        if (interations >= maxInterations)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    public void FillBoard(int fallYOffset = 0, float moveTime = 0.1f)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardFiller");
            return;
        }
        int maxStopWhile = 100;
        int numStopWhile = 0;

        for (int i = 0; i < Board.Width; i++)
        {
            for (int j = 0; j < Board.Height; j++)
            {
                if (Board.AllGamePieces[i, j] == null && Board.AllTiles[i, j].TileType != TileType.Obstacle)
                {
                    if (j == (Board.Height - 1) && Board.BoardQuery.CanAddCollectible())
                    {
                        this.FillRandomCollectibleAt(i, j, fallYOffset, moveTime);
                        Board.CollectibleCount++;
                    }
                    else
                    {
                        this.FillRandomGamePieceAt(i, j, fallYOffset, moveTime);
                        numStopWhile = 0;
                        while (Board.BoardQuery.HasMatchOnFill(i, j))
                        {
                            Board.BoardClearer.ClearPieceAt(i, j);
                            this.FillRandomGamePieceAt(i, j, fallYOffset, moveTime);
                            numStopWhile++;
                            if (numStopWhile >= maxStopWhile)
                            {
                                Debug.Log("While Break ========================");
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private GamePiece FillRandomGamePieceAt(int x, int y, int fallYOffset = 0, float timeMove = 0.1f)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardFiller");
            return null;
        }
        if (Board.BoardQuery.IsWithinBounds(x, y))
        {
            GamePiece randomPiece = Instantiate(Board.BoardQuery.GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GamePiece;
            this.MakeGamePiece(randomPiece, x, y, fallYOffset, timeMove);
            return randomPiece;
        }
        return null;
    }

    private GamePiece FillRandomCollectibleAt(int x, int y, int fallYOffset = 0, float timeMove = 0.1f)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardFiller");
            return null;
        }
        if (Board.BoardQuery.IsWithinBounds(x, y))
        {
            GamePiece randomPiece = Instantiate(Board.BoardQuery.GetRandomCollectible(), Vector3.zero, Quaternion.identity) as GamePiece;
            this.MakeGamePiece(randomPiece, x, y, fallYOffset, timeMove);
            return randomPiece;
        }
        return null;
    }

    public void MakeTile(Tile tilePrefab, int x, int y, int z = 0)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardFiller");
            return;
        }
        if (tilePrefab != null && Board.BoardQuery.IsWithinBounds(x, y))
        {
            Tile tile = Instantiate(tilePrefab, new Vector3(x, y, z), Quaternion.identity) as Tile;
            tile.name = $"Tile {x} - {y}";
            tile.transform.parent = transform;
            Board.AllTiles[x, y] = tile;
            Board.AllTiles[x, y].Init(x, y, Board);
        }
    }
    public void MakeGamePiece(GamePiece gamePiecePrefab, int x, int y, int fallYOffset = 0, float timeMove = 0.1f)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardFiller");
            return;
        }
        if (gamePiecePrefab != null && Board.BoardQuery.IsWithinBounds(x, y))
        {
            gamePiecePrefab.Init(Board);
            this.PlaceGamePiece(gamePiecePrefab, x, y);
            if (fallYOffset != 0)
            {
                gamePiecePrefab.transform.position = new Vector3(x, y + fallYOffset, 0);
                gamePiecePrefab.Move(x, y, timeMove);
            }
            gamePiecePrefab.transform.parent = this.transform;
        }
    }

    public Bomb MakeBomb(GamePiece gamePiece, int x, int y)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardFiller");
            return null;
        }
        if (gamePiece)
        {
            Bomb bombPrefab = gamePiece.GetComponent<Bomb>();
            if (bombPrefab != null && Board.BoardQuery.IsWithinBounds(x, y))
            {
                Bomb bomb = Instantiate(bombPrefab, new Vector3(x, y, 0), Quaternion.identity);
                bomb.Init(Board);
                bomb.SetCoord(x, y);
                bomb.transform.parent = this.transform;
                return bomb;
            }
        }

        return null;
    }

    public void MakeColorBombBooster(int x, int y)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardFiller");
            return;
        }
        if (Board.BoardQuery.IsWithinBounds(x, y))
        {
            GamePiece pieceToReplace = Board.AllGamePieces[x, y];
            if (pieceToReplace != null)
            {
                Board.BoardClearer.ClearPieceAt(x, y);
                Bomb bomb = this.MakeBomb(Board.ColorBombPrefab, x, y);
                Board.BoardBomber.ActivateBomb(bomb);
            }
        }
    }

    public IEnumerator RefillRoutine()
    {
        this.FillBoard(Board.FillYOffset, Board.FillMoveTime);
        yield return null;
    }
}
