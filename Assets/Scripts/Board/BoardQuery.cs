using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardQuery : MonoBehaviour
{
    public Board Board;
    private void Awake()
    {
        Board = GetComponent<Board>();
    }

    public T GetRandomObject<T>(T[] objectArray)
    {
        int randomIdx = Random.Range(0, objectArray.Length);
        if (objectArray[randomIdx] == null)
        {
            Debug.LogWarning($"BOARD.GetRandomObject at index {randomIdx} does not contain a valid GameObject");
        }
        return objectArray[randomIdx];
    }

    public GamePiece GetRandomGamePiece()
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardQuery");
            return null;
        }
        return this.GetRandomObject(Board.ListGamepiecePrefab);
    }

    public Collectible GetRandomCollectible()
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardQuery");
            return null;
        }
        return this.GetRandomObject(Board.ListCollectiblePrefab);
    }
    public List<int> GetColumns(List<GamePiece> listgamePiece)
    {
        List<int> columns = new List<int>();
        foreach (GamePiece piece in listgamePiece)
        {
            if (piece != null)
            {
                if (!columns.Contains(piece.xIndex))
                {
                    columns.Add(piece.xIndex);
                }

            }
        }
        return columns;
    }

    public List<GamePiece> GetRowPieces(int row)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardQuery");
            return null;
        }
        List<GamePiece> listGamePiece = new List<GamePiece>();
        for (int i = 0; i < Board.Width; i++)
        {
            if (Board.AllGamePieces[i, row] != null)
            {
                listGamePiece.Add(Board.AllGamePieces[i, row]);
            }
        }
        return listGamePiece;
    }

    public List<GamePiece> GetComlumnPieces(int column)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardQuery");
            return null;
        }
        List<GamePiece> listGamePiece = new List<GamePiece>();
        for (int i = 0; i < Board.Height; i++)
        {
            if (Board.AllGamePieces[column, i] != null)
            {
                listGamePiece.Add(Board.AllGamePieces[column, i]);
            }
        }
        return listGamePiece;
    }

    private List<GamePiece> GetAdjacentPieces(int x, int y, int offset = 1)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardQuery");
            return null;
        }
        List<GamePiece> listGamePiece = new List<GamePiece>();
        for (int i = x - offset; i <= x + offset; i++)
        {
            for (int j = y - offset; j <= y + offset; j++)
            {
                if (this.IsWithinBounds(i, j))
                {
                    listGamePiece.Add(Board.AllGamePieces[i, j]);
                }
            }
        }
        return listGamePiece;
    }

    public List<GamePiece> GetBombedPieces(List<GamePiece> listGamePiece)
    {
        List<GamePiece> allPieceToClear = new List<GamePiece>();
        foreach (GamePiece gamePiece in listGamePiece)
        {
            if (gamePiece != null)
            {
                List<GamePiece> pieceToClear = new List<GamePiece>();
                Bomb bomb = gamePiece.GetComponent<Bomb>();
                if (bomb != null)
                {
                    switch (bomb.BombType)
                    {
                        case BombType.Column:
                            pieceToClear = this.GetComlumnPieces(bomb.xIndex);
                            break;
                        case BombType.Row:
                            pieceToClear = this.GetRowPieces(bomb.yIndex);
                            break;
                        case BombType.Adjacent:
                            pieceToClear = this.GetAdjacentPieces(bomb.xIndex, bomb.yIndex, 1);
                            break;
                        case BombType.Color:
                            break;
                    }
                    allPieceToClear = allPieceToClear.Union(pieceToClear).ToList();
                    allPieceToClear = this.RemoveCollectibles(allPieceToClear);
                }
            }
        }
        return allPieceToClear;
    }

    public List<GamePiece> GetCollectedPieces(List<GamePiece> listgamePiece)
    {
        List<GamePiece> collectedPieces = this.FindCollectiblesAt(0, true);
        List<GamePiece> allCollectibles = this.FindAllCollectibles();
        List<GamePiece> blocker = listgamePiece.Intersect(allCollectibles).ToList();
        collectedPieces = collectedPieces.Union(blocker).ToList();
        return collectedPieces;
    }
    public List<GamePiece> RemoveCollectibles(List<GamePiece> listbombedPieces)
    {
        List<GamePiece> listCollectiblePiece = this.FindAllCollectibles();
        List<GamePiece> pieceToRemove = new List<GamePiece>();
        foreach (GamePiece piece in listCollectiblePiece)
        {
            Collectible collectible = piece.GetComponent<Collectible>();
            if (collectible != null)
            {
                if (!collectible.ClearedByBomb)
                {
                    pieceToRemove.Add(piece);
                }
            }
        }
        return listbombedPieces.Except(pieceToRemove).ToList();
    }

    public bool IsWithinBounds(int x, int y)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardQuery");
            return false;
        }
        return (x >= 0 && x < Board.Width && y >= 0 && y < Board.Height);
    }

    public bool IsCornerMatch(List<GamePiece> listGamePiece)
    {
        bool vertical = false;
        bool horizontal = false;
        int Xstart = -1;
        int Ystart = -1;
        foreach (GamePiece gamePiece in listGamePiece)
        {
            if (Xstart == -1 || Ystart == -1)
            {
                Xstart = gamePiece.xIndex;
                Ystart = gamePiece.yIndex;
            }

            if (gamePiece.xIndex == Xstart && gamePiece.yIndex != Ystart)
            {
                vertical = true;
            }
            if (gamePiece.xIndex != Xstart && gamePiece.yIndex == Ystart)
            {
                horizontal = true;
            }
        }
        return vertical && horizontal;
    }

    public bool IsColorBomb(GamePiece gamePiece)
    {
        Bomb bomb = gamePiece.GetComponent<Bomb>();
        if (bomb != null)
        {
            return (bomb.BombType == BombType.Color);
        }
        return false;
    }

    public bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex)
        {
            return true;
        }
        if (Mathf.Abs(start.yIndex - end.yIndex) == 1 && start.xIndex == end.xIndex)
        {
            return true;
        }
        return false;
    }

    public bool IsCollapsed(List<GamePiece> listgamePiece)
    {
        foreach (GamePiece piece in listgamePiece)
        {
            if (piece != null)
            {
                if (piece.transform.position.y - piece.yIndex > 0.01f)
                {
                    return false;
                }
                if (piece.transform.position.x - piece.xIndex > 0.01f)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardQuery");
            return false;
        }
        List<GamePiece> leftMatches = Board.BoardMatcher.FindMatches(x, y, new Vector2(-1, 0), minLength);
        List<GamePiece> downTowardMatches = Board.BoardMatcher.FindMatches(x, y, new Vector2(0, -1), minLength);

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }
        if (downTowardMatches == null)
        {
            downTowardMatches = new List<GamePiece>();
        }
        return (leftMatches.Count > 0 || downTowardMatches.Count > 0);
    }

    public bool CanAddCollectible()
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardQuery");
            return false;
        }
        return (Random.Range(0f, 1f) <= Board.ChanceForCollectible && Board.ListCollectiblePrefab.Length > 0 && Board.CollectibleCount < Board.MaxCollectibles);
    }

    public List<GamePiece> FindCollectiblesAt(int row, bool clearAtBottom = false)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardQuery");
            return null;
        }
        List<GamePiece> foundCollectibles = new List<GamePiece>();
        for (int i = 0; i < Board.Width; i++)
        {
            if (Board.AllGamePieces[i, row] != null)
            {
                Collectible collectible = Board.AllGamePieces[i, row].GetComponent<Collectible>();
                if (collectible != null)
                {
                    if (!clearAtBottom || (clearAtBottom && collectible.ClearedAtBottom))
                    {
                        foundCollectibles.Add(collectible);
                    }

                }
            }
        }
        return foundCollectibles;
    }

    public List<GamePiece> FindAllCollectibles(bool clearAtBottom = false)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardQuery");
            return null;
        }
        List<GamePiece> foundCollectibles = new List<GamePiece>();
        for (int i = 0; i < Board.Height; i++)
        {
            List<GamePiece> collectiblesRow = this.FindCollectiblesAt(i, clearAtBottom);
            foundCollectibles = foundCollectibles.Union(collectiblesRow).ToList();
        }
        return foundCollectibles;
    }

    public MatchValue FindMatchValue(List<GamePiece> listGamePiece)
    {
        foreach (GamePiece gamePiece in listGamePiece)
        {
            if (gamePiece != null)
            {
                return gamePiece.MatchValue;
            }
        }
        return MatchValue.None;
    }

    public GamePiece FindGamePieceByMatchValue(GamePiece[] listgamePiecePrefab, MatchValue matchValue)
    {
        if (matchValue == MatchValue.None)
        {
            return null;
        }

        foreach (GamePiece gamePiece in listgamePiecePrefab)
        {
            if (gamePiece != null)
            {
                if (gamePiece.MatchValue == matchValue)
                {
                    return gamePiece;
                }
            }
        }
        return null;
    }

    public List<GamePiece> FindAllPieceByMatchValue(MatchValue matchValue)
    {
        if (Board == null)
        {
            Debug.LogWarning("BOARD IS INVALID IN BoardQuery");
            return null;
        }
        List<GamePiece> foundPieces = new List<GamePiece>();
        for (int i = 0; i < Board.Width; i++)
        {
            for (int j = 0; j < Board.Height; j++)
            {
                if (Board.AllGamePieces[i, j] != null)
                {
                    if (Board.AllGamePieces[i, j].MatchValue == matchValue)
                    {
                        foundPieces.Add(Board.AllGamePieces[i, j]);
                    }
                }
            }
        }
        return foundPieces;
    }
}
