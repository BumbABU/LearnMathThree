using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(BoardDeadLock))]
[RequireComponent(typeof(BoardShuffler))]
public class Board : MonoBehaviour
{
    public int Width;
    public int Height;
    public int BorderSize;
    public float SwapTime = 0.5f;
    public int FillYOffset = 10;
    public float FillMoveTime = 0.5f;
    public bool IsRefilling = false;

    public Tile NormalTile;
    public Tile ObstacleTile;
    public GamePiece[] ListGamepiecePrefab;
    public StartingObject[] ListStartingTiles;
    public StartingObject[] ListStartingGamePiece;

    //bomb prefab
    public Bomb[] ListColumnBombPrefab;
    public Bomb[] ListRowBombPrefab;
    public Bomb[] ListAdjacentBombPrefab;
    public Bomb ColorBombPrefab;

    //collecible numerical
    public int MaxCollectibles = 3;
    public int CollectibleCount = 0;
    [Range(0, 1)]
    public float ChanceForCollectible = 0.1f;
    public Collectible[] ListCollectiblePrefab;



    [SerializeField] private int _scoreMultiplier = 0;
    private Tile[,] _allTiles;
    private GamePiece[,] _allGamePieces;
    private Tile _clickedTile;
    private Tile _targetTile;
    private ParticleManager _particleManager;
    private Bomb _clickedTileBomb;
    private Bomb _targetTileBomb;

    private BoardDeadLock _boardDeadLock;
    private BoardShuffler _boardShuffler;


    [System.Serializable]
    public class StartingObject
    {
        public GameObject Prefab;
        public int x;
        public int y;
        public int z;
    }

    [SerializeField] private bool _playerInputEnable = true;
    private void Start()
    {
        this._allTiles = new Tile[Width, Height];
        this._allGamePieces = new GamePiece[Width, Height];
        if (this._particleManager == null)
        {
            this._particleManager = GameObject.FindGameObjectWithTag("ParticleManager").GetComponent<ParticleManager>();
        }
        this._boardDeadLock = GetComponent<BoardDeadLock>();
        this._boardShuffler = GetComponent<BoardShuffler>();
    }

    public void SetUpBoard()
    {
        this.SetupCameara();
        this.SetupTiles();
        this.SetupGamePiece();
        List<GamePiece> listStartingCollectible = this.FindAllCollectibles();
        this.CollectibleCount = listStartingCollectible.Count;
        this.FillBoard(FillYOffset, FillMoveTime);
    }

    private void SetupCameara()
    {
        Camera.main.transform.position = new Vector3((this.Width - 1) / 2, (this.Height - 1) / 2, -10);
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float horizontalSize = ((float)this.Width / 2 + (float)this.BorderSize) / aspectRatio;
        float verticalSize = (float)this.Height / 2 + (float)this.BorderSize;
        Camera.main.orthographicSize = verticalSize > horizontalSize ? verticalSize : horizontalSize;
    }
    private void SetupTiles()
    {
        foreach (StartingObject startingTile in ListStartingTiles)
        {
            if (startingTile != null)
            {
                this.MakeTile(startingTile.Prefab.GetComponent<Tile>(), startingTile.x, startingTile.y, startingTile.z);
            }
        }

        for (int i = 0; i < this.Width; i++)
        {
            for (int j = 0; j < this.Height; j++)
            {
                if (_allTiles[i, j] == null)
                {
                    this.MakeTile(this.NormalTile, i, j);
                }
            }
        }
    }


    private void MakeTile(Tile tilePrefab, int x, int y, int z = 0)
    {
        if (tilePrefab != null && this.IsWithinBounds(x, y))
        {
            Tile tile = Instantiate(tilePrefab, new Vector3(x, y, z), Quaternion.identity) as Tile;
            tile.name = $"Tile {x} - {y}";
            tile.transform.parent = transform;
            _allTiles[x, y] = tile;
            _allTiles[x, y].Init(x, y, this);
        }
    }
    private void MakeGamePiece(GamePiece gamePiecePrefab, int x, int y, int fallYOffset = 0, float timeMove = 0.1f)
    {
        if (gamePiecePrefab != null && this.IsWithinBounds(x, y))
        {
            gamePiecePrefab.Init(this);
            this.PlaceGamePiece(gamePiecePrefab, x, y);
            if (fallYOffset != 0)
            {
                gamePiecePrefab.transform.position = new Vector3(x, y + fallYOffset, 0);
                gamePiecePrefab.Move(x, y, timeMove);
            }
            gamePiecePrefab.transform.parent = this.transform;
        }
    }

    private Bomb MakeBomb(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece)
        {
            Bomb bombPrefab = gamePiece.GetComponent<Bomb>();
            if (bombPrefab != null && this.IsWithinBounds(x, y))
            {
                Bomb bomb = Instantiate(bombPrefab, new Vector3(x, y, 0), Quaternion.identity);
                bomb.Init(this);
                bomb.SetCoord(x, y);
                bomb.transform.parent = this.transform;
                return bomb;
            }
        }

        return null;
    }

    public void MakeColorBombBooster(int x, int y)
    {
        if (this.IsWithinBounds(x, y))
        {
            GamePiece pieceToReplace = this._allGamePieces[x, y];
            if (pieceToReplace != null)
            {
                this.ClearPieceAt(x, y);
                Bomb bomb = this.MakeBomb(this.ColorBombPrefab, x, y);
                this.ActivateBomb(bomb);
            }
        }
    }

    private void SetupGamePiece()
    {
        foreach (StartingObject sGamePiece in ListStartingGamePiece)
        {
            if (sGamePiece != null)
            {
                GamePiece gamePiece = Instantiate(sGamePiece.Prefab.GetComponent<GamePiece>(), new Vector3(sGamePiece.x, sGamePiece.y, 0), Quaternion.identity) as GamePiece;
                this.MakeGamePiece(gamePiece, sGamePiece.x, sGamePiece.y, this.FillYOffset, this.FillMoveTime);
            }
        }
    }

    private T GetRandomObject<T>(T[] objectArray)
    {
        int randomIdx = Random.Range(0, objectArray.Length);
        if (objectArray[randomIdx] == null)
        {
            Debug.LogWarning($"BOARD.GetRandomObject at index {randomIdx} does not contain a valid GameObject");
        }
        return objectArray[randomIdx];
    }

    private GamePiece GetRandomGamePiece()
    {
        return this.GetRandomObject(ListGamepiecePrefab);
    }

    private Collectible GetRandomCollectible()
    {
        return this.GetRandomObject(this.ListCollectiblePrefab);
    }

    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning("BOARD: Invalid GamePiece");
            return;
        }
        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;
        if (IsWithinBounds(x, y))
        {
            this._allGamePieces[x, y] = gamePiece;
        }
        gamePiece.SetCoord(x, y);
    }

    public void FillBoardFromList(List<GamePiece> listAllPiece)
    {
        Queue<GamePiece> unusedGamePiece = new Queue<GamePiece>(listAllPiece);

        int maxInterations = 100;
        int interations = 0;

        for (int i = 0; i < this.Width; i++)
        {
            for (int j = 0; j < this.Height; j++)
            {
                if (this._allGamePieces[i, j] == null && this._allTiles[i, j].TileType != TileType.Obstacle)
                {
                    this._allGamePieces[i, j] = unusedGamePiece.Dequeue();
                    interations = 0;
                    while (this.HasMatchOnFill(j, j))
                    {
                        interations++;
                        unusedGamePiece.Enqueue(this._allGamePieces[i, j]);
                        this._allGamePieces[i, j] = unusedGamePiece.Dequeue();
                        if (interations >= maxInterations)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private void FillBoard(int fallYOffset = 0, float moveTime = 0.1f)
    {
        int maxStopWhile = 100;
        int numStopWhile = 0;

        for (int i = 0; i < this.Width; i++)
        {
            for (int j = 0; j < this.Height; j++)
            {
                if (_allGamePieces[i, j] == null && _allTiles[i, j].TileType != TileType.Obstacle)
                {
                    if (j == (this.Height - 1) && this.CanAddCollectible())
                    {
                        this.FillRandomCollectibleAt(i, j, fallYOffset, moveTime);
                        this.CollectibleCount++;
                    }
                    else
                    {
                        this.FillRandomGamePieceAt(i, j, fallYOffset, moveTime);
                        numStopWhile = 0;
                        while (HasMatchOnFill(i, j))
                        {
                            this.ClearPieceAt(i, j);
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
        if (IsWithinBounds(x, y))
        {
            GamePiece randomPiece = Instantiate(this.GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GamePiece;
            this.MakeGamePiece(randomPiece, x, y, fallYOffset, timeMove);
            return randomPiece;
        }
        return null;
    }

    private GamePiece FillRandomCollectibleAt(int x, int y, int fallYOffset = 0, float timeMove = 0.1f)
    {
        if (IsWithinBounds(x, y))
        {
            GamePiece randomPiece = Instantiate(this.GetRandomCollectible(), Vector3.zero, Quaternion.identity) as GamePiece;
            this.MakeGamePiece(randomPiece, x, y, fallYOffset, timeMove);
            return randomPiece;
        }
        return null;
    }

    private bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        List<GamePiece> leftMatches = this.FindMatches(x, y, new Vector2(-1, 0), minLength);
        List<GamePiece> downTowardMatches = this.FindMatches(x, y, new Vector2(0, -1), minLength);

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

    private bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < this.Width && y >= 0 && y < this.Height);
    }

    public void ClickTile(Tile tile)
    {
        if (this._clickedTile == null)
        {
            this._clickedTile = tile;
        }
    }

    public void DragToTile(Tile tile)
    {
        if (this._clickedTile != null && this.IsNextTo(tile, _clickedTile))
        {
            this._targetTile = tile;
        }
        else
        {
            this._targetTile = null;
        }
    }

    public void ReleaseTile()
    {
        if (this._clickedTile != null && this._targetTile != null)
        {
            this.SwitchTile(this._clickedTile, this._targetTile);
        }
        this._clickedTile = null;
        this._targetTile = null;
    }

    private void SwitchTile(Tile clickedTile, Tile targetTile)
    {
        StartCoroutine(SwitchTileRoutine(clickedTile, targetTile));
    }

    private IEnumerator SwitchTileRoutine(Tile clickedTile, Tile targetTile)
    {
        if (_playerInputEnable && !GameManager.Instance.IsGameOver)
        {
            GamePiece clickedPiece = this._allGamePieces[clickedTile.xIndex, clickedTile.yIndex];
            GamePiece targetPiece = this._allGamePieces[targetTile.xIndex, targetTile.yIndex];
            if (clickedPiece != null && targetPiece != null)
            {
                clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, SwapTime);
                targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, SwapTime);

                yield return new WaitForSeconds(SwapTime);
                List<GamePiece> clickedPieceMatches = this.FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
                List<GamePiece> targetPieceMatches = this.FindMatchesAt(targetTile.xIndex, targetTile.yIndex);
                #region ColorBomb
                List<GamePiece> colorMatches = this.ProcessColorBomb(clickedPiece, targetPiece);
                #endregion

                if (clickedPieceMatches.Count == 0 && targetPieceMatches.Count == 0 && colorMatches.Count == 0)
                {
                    clickedPiece.Move(clickedTile.xIndex, clickedTile.yIndex, SwapTime);
                    targetPiece.Move(targetTile.xIndex, targetTile.yIndex, SwapTime);
                }
                else
                {
                    yield return new WaitForSeconds(SwapTime);

                    #region DropBombs
                    Vector2 swipeDirection = new Vector2(targetTile.xIndex - clickedTile.xIndex, targetTile.yIndex - clickedTile.yIndex);
                    this._clickedTileBomb = this.DropBomb(clickedTile.xIndex, clickedTile.yIndex, swipeDirection, clickedPieceMatches);
                    this._targetTileBomb = this.DropBomb(targetTile.xIndex, targetTile.yIndex, swipeDirection, targetPieceMatches);

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
                    }*/ // oldversion (chang    e Color)
                    #endregion

                    List<GamePiece> combinedListPieceMatches = clickedPieceMatches.Union(targetPieceMatches).ToList().Union(colorMatches).ToList();
                    yield return StartCoroutine(this.ClearAndRefillBoardRoutine(combinedListPieceMatches));
                    if (GameManager.Instance)
                    {
                        GameManager.Instance.UpdateMoveLeft();
                    }
                }
            }
        }

    }

    private List<GamePiece> ProcessColorBomb(GamePiece clickedPiece, GamePiece targetPiece, bool clearNonBlockers = false )
    {
        List<GamePiece> colorMatches = new List<GamePiece>();
        GamePiece colorBombPiece = null;
        GamePiece otherPiece = null;
        if (this.IsColorBomb(clickedPiece) && !this.IsColorBomb(targetPiece))
        {
            colorBombPiece = clickedPiece;
            otherPiece = targetPiece;
            /*            clickedPiece.MatchValue = targetPiece.MatchValue;
                        colorMatches = this.FindAllPieceByMatchValue(clickedPiece.MatchValue);*/
        }
        else if (!this.IsColorBomb(clickedPiece) && this.IsColorBomb(targetPiece))
        {
            colorBombPiece = targetPiece;
            otherPiece = clickedPiece;
            /*            targetPiece.MatchValue = clickedPiece.MatchValue;
                        colorMatches = this.FindAllPieceByMatchValue(targetPiece.MatchValue);*/
        }
        else if (this.IsColorBomb(clickedPiece) && this.IsColorBomb(targetPiece))
        {
            foreach (GamePiece gamePiece in _allGamePieces)
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
            colorMatches = this.FindAllPieceByMatchValue(colorBombPiece.MatchValue);
        }
        if (!clearNonBlockers)
        {
            List<GamePiece> collectedAtbottom = this.FindAllCollectibles(true);
            if(collectedAtbottom.Contains(otherPiece))
            {
                return new List<GamePiece>();
            }
            else
            {
                foreach (GamePiece piece in collectedAtbottom)
                {
                    if(colorMatches.Contains(piece))
                    {
                        colorMatches.Remove(piece);
                    }
                }
            }
        }

        return colorMatches;
    }

    private bool IsNextTo(Tile start, Tile end)
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

    private List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        GamePiece startPiece = null;
        if (IsWithinBounds(startX, startY))
        {
            startPiece = _allGamePieces[startX, startY];
        }
        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }
        int nextX;
        int nextY;

        int maxValue = (this.Width > this.Height) ? this.Width : this.Height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }
            GamePiece nextPiece = _allGamePieces[nextX, nextY];

            if (nextPiece == null)
            {
                break;
            }
            else
            {
                if (nextPiece.MatchValue == startPiece.MatchValue && !matches.Contains(nextPiece) && nextPiece.MatchValue != MatchValue.None)
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }

        if (matches.Count >= minLength)
        {
            return matches;
        }
        return null;
    }

    private List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = this.FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<GamePiece> downwardMatches = this.FindMatches(startX, startY, new Vector2(0, -1), 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }
        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        //option1 default

        /*foreach (GamePiece gamePiece in downwardMatches)
        {
            if (!upwardMatches.Contains(gamePiece))
            {
                upwardMatches.Add(gamePiece);
            }
        }

        return (upwardMatches.Count >= minLength) ? upwardMatches : null;*/

        //option2 Linq
        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();
        return (combinedMatches.Count >= minLength) ? combinedMatches : null;
    }

    private List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> rightMatches = this.FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<GamePiece> leftMatches = this.FindMatches(startX, startY, new Vector2(-1, 0), 2);

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();
        return (combinedMatches.Count >= minLength) ? combinedMatches : null;
    }

    private List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<GamePiece> horizontalMatches = this.FindHorizontalMatches(x, y, minLength);
        List<GamePiece> verticalMatches = this.FindVerticalMatches(x, y, minLength);
        if (horizontalMatches == null)
        {
            horizontalMatches = new List<GamePiece>();
        }
        if (verticalMatches == null)
        {
            verticalMatches = new List<GamePiece>();
        }

        var combinedMatches = verticalMatches.Union(horizontalMatches).ToList();
        return combinedMatches;
    }

    private List<GamePiece> FindMatchesAt(List<GamePiece> listgamePiece, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        foreach (GamePiece piece in listgamePiece)
        {
            matches = matches.Union(FindMatchesAt(piece.xIndex, piece.yIndex, minLength)).ToList();
        }
        return matches;
    }

    private List<GamePiece> FindAllMatches()
    {
        List<GamePiece> combinedMatches = new List<GamePiece>();
        for (int i = 0; i < this.Width; i++)
        {
            for (int j = 0; j < this.Height; j++)
            {
                List<GamePiece> matches = this.FindMatchesAt(i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }
        return combinedMatches;
    }

    public void HighLightMatches() // for test
    {
        for (int i = 0; i < this.Width; i++)
        {
            for (int j = 0; j < this.Height; j++)
            {
                HighLightMatchesAt(i, j);
            }
        }
    }

    private void HighLightMatchesAt(int x, int y)
    {
        this.HighLightTileOff(x, y);
        var combinedMatches = FindMatchesAt(x, y);
        if (combinedMatches.Count > 0)
        {
            foreach (GamePiece gamePiece in combinedMatches)
            {
                HighLightTileOn(gamePiece.xIndex, gamePiece.yIndex, gamePiece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    private void HighLightTileOn(int x, int y, Color color)
    {
        if (_allTiles[x, y].TileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = _allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
        }
    }

    private void HighLightTileOff(int x, int y)
    {
        if (_allTiles[x, y].TileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = _allTiles[x, y].GetComponent<SpriteRenderer>();
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

    private void ClearPieceAt(int x, int y)
    {
        GamePiece gamePieceToClear = _allGamePieces[x, y];
        if (gamePieceToClear != null)
        {
            _allGamePieces[x, y] = null;
            Destroy(gamePieceToClear.gameObject);
        }
        // HighLightTileOff(x, y);
    }

    private void ClearPieceAt(List<GamePiece> listgamePiece, List<GamePiece> listBomb)
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
                    GameManager.Instance.ScorePoints(gamePiece, this._scoreMultiplier, bonus);
                    TimeBonus timeBonus = gamePiece.GetComponent<TimeBonus>();
                    if (timeBonus != null)
                    {
                        GameManager.Instance.AddTime(timeBonus.BonusValue);
                        Debug.Log("Add time bonus: " + timeBonus.BonusValue);
                    }
                    GameManager.Instance.UpdateCollectionsGoals(gamePiece);
                }
                if (_particleManager != null)
                {
                    if (listBomb.Contains(gamePiece))
                    {
                        _particleManager.BombFXAt(gamePiece.xIndex, gamePiece.yIndex);
                    }
                    else
                    {
                        _particleManager.ClearPieceFXAt(gamePiece.xIndex, gamePiece.yIndex);
                    }
                }
            }
        }
    }

    private void ClearBoard()
    {
        for (int i = 0; i < this.Width; i++)
        {
            for (int j = 0; j < this.Height; j++)
            {
                this.ClearPieceAt(i, j);
                if (this._particleManager != null)
                {
                    this._particleManager.ClearPieceFXAt(i, j);
                }
            }
        }
    }

    private void BreakTileAt(int x, int y)
    {
        Tile tiletoBreak = _allTiles[x, y];
        if (tiletoBreak != null && tiletoBreak.TileType == TileType.Breakable)
        {
            if (_particleManager != null)
            {
                _particleManager.BreakTileFXAt(tiletoBreak.BreakableValue, x, y, 0);
            }
            tiletoBreak.BreakTile();
        }
    }

    private void BreakTileAt(List<GamePiece> listgamePiece)
    {
        foreach (GamePiece piece in listgamePiece)
        {
            if (piece != null)
            {
                BreakTileAt(piece.xIndex, piece.yIndex);
            }
        }
    }

    private List<GamePiece> CollapseColumn(int column, float collapseTime = 0.1f) // input (number of column) => out put (game piece to collape)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        for (int i = 0; i < this.Height - 1; i++)
        {
            if (_allGamePieces[column, i] == null && this._allTiles[column, i].TileType != TileType.Obstacle)
            {
                for (int j = i + 1; j < this.Height; j++)
                {
                    if (this._allGamePieces[column, j] != null)
                    {
                        this._allGamePieces[column, j].Move(column, i, collapseTime * (j - i));
                        this._allGamePieces[column, i] = this._allGamePieces[column, j];
                        this._allGamePieces[column, i].SetCoord(column, i);
                        if (!movingPieces.Contains(_allGamePieces[column, i]))
                        {
                            movingPieces.Add(_allGamePieces[column, i]);
                        }
                        this._allGamePieces[column, j] = null;
                        break;
                    }
                }
            }
        }
        return movingPieces;
    }

    private List<GamePiece> CollapseColumn(List<int> columnsToCollapse)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(this.CollapseColumn(column)).ToList();
        }
        return movingPieces;
    }

    /* private List<GamePiece> CollapseColumn(List<GamePiece> listgamePiece)
     {
         List<GamePiece> movingPieces = new List<GamePiece>();
         List<int> columnsToCollapse = GetColumns(listgamePiece);
         foreach (int column in columnsToCollapse)
         {
             movingPieces = movingPieces.Union(this.CollapseColumn(column)).ToList();
         }
         return movingPieces;
     }*/

    private List<int> GetColumns(List<GamePiece> listgamePiece)
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

    private void ClearAndRefillBoard(List<GamePiece> listgamePiece)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(listgamePiece));
    }

    public void ClearAndRefillBoard(int x, int y) //for zap booster
    {
        if (this.IsWithinBounds(x, y))
        {
            GamePiece piece = this._allGamePieces[x, y];

            List<GamePiece> listonePiece = new List<GamePiece>();
            listonePiece.Add(piece);
            this.ClearAndRefillBoard(listonePiece);
        }
    }

    private IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> listgamePiece)
    {
        this._playerInputEnable = false;
        this.IsRefilling = true;
        List<GamePiece> matches = listgamePiece;
        this._scoreMultiplier = 0;
        do
        {
            this._scoreMultiplier++;
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            yield return null;
            yield return StartCoroutine(RefillRoutine());
            matches = this.FindAllMatches();
            yield return new WaitForSeconds(0.2f);
        }
        while (matches.Count != 0);

        if (this._boardDeadLock.IsDeadLocked(this._allGamePieces))
        {
            yield return new WaitForSeconds(1f);
            //this.ClearBoard();
            yield return StartCoroutine(ShuffleBoardRoutine());
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(RefillRoutine());
        }

        this.IsRefilling = false;
        this._playerInputEnable = true;
    }

    private IEnumerator RefillRoutine()
    {
        this.FillBoard(FillYOffset, FillMoveTime);
        yield return null;
    }
    private IEnumerator ClearAndCollapseRoutine(List<GamePiece> listgamePiece)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<GamePiece> matchesPieces = new List<GamePiece>();
        //  this.HighLightPieces(listgamePiece);

        yield return new WaitForSeconds(0.05f);
        bool isFinished = false;
        while (!isFinished)
        {
            List<GamePiece> bombedPieces = this.GetBombedPieces(listgamePiece);
            listgamePiece = listgamePiece.Union(bombedPieces).ToList();

            bombedPieces = this.GetBombedPieces(listgamePiece);
            listgamePiece = listgamePiece.Union(bombedPieces).ToList();

            List<GamePiece> collectedPieces = this.FindCollectiblesAt(0, true);
            List<GamePiece> allCollectibles = this.FindAllCollectibles();
            List<GamePiece> blocker = listgamePiece.Intersect(allCollectibles).ToList();
            collectedPieces = collectedPieces.Union(blocker).ToList();

            this.CollectibleCount -= collectedPieces.Count;

            listgamePiece = listgamePiece.Union(collectedPieces).ToList();
            List<int> columnsToCollapse = GetColumns(listgamePiece);


            this.ClearPieceAt(listgamePiece, bombedPieces);
            this.BreakTileAt(listgamePiece);

            if (this._clickedTileBomb != null)
            {
                this.ActivateBomb(this._clickedTileBomb);
                this._clickedTileBomb = null;
            }

            if (this._targetTileBomb != null)
            {
                this.ActivateBomb(this._targetTileBomb);
                this._targetTileBomb = null;
            }

            yield return new WaitForSeconds(0.25f);
            movingPieces = this.CollapseColumn(columnsToCollapse);
            while (!IsCollapsed(movingPieces))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
            matchesPieces = this.FindMatchesAt(movingPieces);
            collectedPieces = this.FindCollectiblesAt(0, true);
            matchesPieces = matchesPieces.Union(collectedPieces).ToList();

            if (matchesPieces.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {
                this._scoreMultiplier++;
                if (SoundManager.Instance)
                {
                    SoundManager.Instance.PlayRandomBonusSound();
                }
                yield return StartCoroutine(ClearAndCollapseRoutine(matchesPieces));
                break; // còn nghi vấn
            }
        }
        yield return null;
    }

    private bool IsCollapsed(List<GamePiece> listgamePiece)
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

    private List<GamePiece> GetRowPieces(int row)
    {
        List<GamePiece> listGamePiece = new List<GamePiece>();
        for (int i = 0; i < this.Width; i++)
        {
            if (this._allGamePieces[i, row] != null)
            {
                listGamePiece.Add(_allGamePieces[i, row]);
            }
        }
        return listGamePiece;
    }

    private List<GamePiece> GetComlumnPieces(int column)
    {
        List<GamePiece> listGamePiece = new List<GamePiece>();
        for (int i = 0; i < this.Height; i++)
        {
            if (this._allGamePieces[column, i] != null)
            {
                listGamePiece.Add(_allGamePieces[column, i]);
            }
        }
        return listGamePiece;
    }

    private List<GamePiece> GetAdjacentPieces(int x, int y, int offset = 1)
    {
        List<GamePiece> listGamePiece = new List<GamePiece>();
        for (int i = x - offset; i <= x + offset; i++)
        {
            for (int j = y - offset; j <= y + offset; j++)
            {
                if (this.IsWithinBounds(i, j))
                {
                    listGamePiece.Add(_allGamePieces[i, j]);
                }
            }
        }
        return listGamePiece;
    }

    private List<GamePiece> GetBombedPieces(List<GamePiece> listGamePiece)
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

    private bool IsCornerMatch(List<GamePiece> listGamePiece)
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

    public Bomb DropBomb(int x, int y, Vector2 swapDirection, List<GamePiece> listGamePiece)
    {
        Bomb bomb = null;
        MatchValue matchValue = MatchValue.None;
        if (listGamePiece != null)
        {
            matchValue = this.FindMatchValue(listGamePiece);
        }
        if (listGamePiece.Count >= 5 && matchValue != MatchValue.None)
        {
            if (this.IsCornerMatch(listGamePiece))
            {
                GamePiece adjacentBomb = this.FindGamePieceByMatchValue(ListAdjacentBombPrefab, matchValue);
                if (adjacentBomb != null)
                {
                    bomb = this.MakeBomb(adjacentBomb, x, y);
                }
            }
            else
            {
                if (this.ColorBombPrefab != null)
                {
                    bomb = this.MakeBomb(this.ColorBombPrefab, x, y);
                }
            }

        }
        else if (listGamePiece.Count >= 4)
        {
            if (swapDirection.x != 0)
            {
                GamePiece rowBomb = this.FindGamePieceByMatchValue(ListRowBombPrefab, matchValue);
                if (rowBomb != null)
                {
                    bomb = this.MakeBomb(rowBomb, x, y);
                }
            }
            else
            {
                GamePiece comlumnBomb = this.FindGamePieceByMatchValue(ListColumnBombPrefab, matchValue);
                if (comlumnBomb != null)
                {
                    bomb = this.MakeBomb(comlumnBomb, x, y);
                }
            }
        }
        return bomb;
    }

    private void ActivateBomb(Bomb bomb)
    {
        int x = (int)bomb.transform.position.x;
        int y = (int)bomb.transform.position.y;
        if (this.IsWithinBounds(x, y))
        {
            _allGamePieces[x, y] = bomb;
        }
    }

    private List<GamePiece> FindAllPieceByMatchValue(MatchValue matchValue)
    {
        List<GamePiece> foundPieces = new List<GamePiece>();
        for (int i = 0; i < this.Width; i++)
        {
            for (int j = 0; j < this.Height; j++)
            {
                if (this._allGamePieces[i, j] != null)
                {
                    if (this._allGamePieces[i, j].MatchValue == matchValue)
                    {
                        foundPieces.Add(this._allGamePieces[i, j]);
                    }
                }
            }
        }
        return foundPieces;
    }

    private bool IsColorBomb(GamePiece gamePiece)
    {
        Bomb bomb = gamePiece.GetComponent<Bomb>();
        if (bomb != null)
        {
            return (bomb.BombType == BombType.Color);
        }
        return false;
    }

    private List<GamePiece> FindCollectiblesAt(int row, bool clearAtBottom = false)
    {
        List<GamePiece> foundCollectibles = new List<GamePiece>();
        for (int i = 0; i < Width; i++)
        {
            if (_allGamePieces[i, row] != null)
            {
                Collectible collectible = _allGamePieces[i, row].GetComponent<Collectible>();
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

    private List<GamePiece> FindAllCollectibles(bool clearAtBottom = false)
    {
        List<GamePiece> foundCollectibles = new List<GamePiece>();
        for (int i = 0; i < this.Height; i++)
        {
            List<GamePiece> collectiblesRow = this.FindCollectiblesAt(i, clearAtBottom);
            foundCollectibles = foundCollectibles.Union(collectiblesRow).ToList();
        }
        return foundCollectibles;
    }

    private bool CanAddCollectible()
    {
        return (Random.Range(0f, 1f) <= this.ChanceForCollectible && this.ListCollectiblePrefab.Length > 0 && this.CollectibleCount < this.MaxCollectibles);
    }

    private List<GamePiece> RemoveCollectibles(List<GamePiece> listbombedPieces)
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

    private MatchValue FindMatchValue(List<GamePiece> listGamePiece)
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

    private GamePiece FindGamePieceByMatchValue(GamePiece[] listgamePiecePrefab, MatchValue matchValue)
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

    public void CheckDeadLock()
    {
        this._boardDeadLock.IsDeadLocked(_allGamePieces);
    }

    public void ShuffleBoard()
    {
        if (this._playerInputEnable)
        {
            StartCoroutine(ShuffleBoardRoutine());
        }
    }

    private IEnumerator ShuffleBoardRoutine()
    {
        List<GamePiece> listAllGamePiece = new List<GamePiece>();
        foreach (GamePiece piece in this._allGamePieces)
        {
            listAllGamePiece.Add(piece);
        }

        while (!this.IsCollapsed(listAllGamePiece))
        {
            yield return null;
        }

        List<GamePiece> normalPieces = this._boardShuffler.RemoveNormalPieces(this._allGamePieces);
        this._boardShuffler.ShuffleList(normalPieces);
        this.FillBoardFromList(normalPieces);
        this._boardShuffler.MovePieces(this._allGamePieces, SwapTime);

        List<GamePiece> matches = this.FindAllMatches();
        StartCoroutine(ClearAndRefillBoardRoutine(matches));
    }
}
