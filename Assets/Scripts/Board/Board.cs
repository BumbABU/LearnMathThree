using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(BoardSetup))]
[RequireComponent(typeof(BoardTiles))]
[RequireComponent(typeof(BoardQuery))]
[RequireComponent(typeof(BoardInput))]
[RequireComponent(typeof(BoardMatcher))]
[RequireComponent(typeof(BoardClearer))]
[RequireComponent(typeof(BoardCollapser))]
[RequireComponent(typeof(BoardFiller))]
[RequireComponent(typeof(BoardBomber))]
[RequireComponent(typeof(BoardShuffler))]
[RequireComponent(typeof(BoardDeadLock))]
[RequireComponent(typeof(BoardHighLighter))]
public class Board : MonoBehaviour
{
    public int Width;
    public int Height;
    public int BorderSize;
    public float SwapTime = 0.5f;
    public int FillYOffset = 10;
    public float FillMoveTime = 0.5f;
    public bool IsRefilling = false;
    public bool IsSwipping = false;

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



    public int ScoreMultiplier = 0;
    public Tile[,] AllTiles;

    public GamePiece[,] AllGamePieces;

    public Tile ClickedTile;
    public Tile TargetTile;
    public ParticleManager ParticleManager;
    public Bomb ClickedTileBomb;
    public Bomb TargetTileBomb;

    //All Board
    public BoardSetup BoardSetup;
    public BoardTiles BoardTiles;
    public BoardQuery BoardQuery;
    public BoardInput BoardInput;
    public BoardMatcher BoardMatcher;
    public BoardClearer BoardClearer;
    public BoardCollapser BoardCollapser;
    public BoardFiller BoardFiller;
    public BoardBomber BoardBomber;
    public BoardDeadLock BoardDeadLock;
    public BoardShuffler BoardShuffler;
    public BoardHighLighter BoardHighLighter;


    [System.Serializable]
    public class StartingObject
    {
        public GameObject Prefab;
        public int x;
        public int y;
        public int z;
    }

    [SerializeField] private bool _playerInputEnable = true;

    private void Awake()
    {
        this.BoardSetup = GetComponent<BoardSetup>();
        this.BoardTiles = GetComponent<BoardTiles>();
        this.BoardQuery = GetComponent<BoardQuery>();
        this.BoardInput = GetComponent<BoardInput>();
        this.BoardMatcher = GetComponent<BoardMatcher>();
        this.BoardClearer = GetComponent<BoardClearer>();
        this.BoardCollapser = GetComponent<BoardCollapser>();
        this.BoardFiller = GetComponent<BoardFiller>();
        this.BoardBomber = GetComponent<BoardBomber>();
        this.BoardDeadLock = GetComponent<BoardDeadLock>();
        this.BoardShuffler = GetComponent<BoardShuffler>();
        this.BoardHighLighter = GetComponent<BoardHighLighter>();
    }

    private void Start()
    {
        this.AllTiles = new Tile[Width, Height];
        this.AllGamePieces = new GamePiece[Width, Height];
        if (this.ParticleManager == null)
        {
            this.ParticleManager = GameObject.FindGameObjectWithTag("ParticleManager").GetComponent<ParticleManager>();
        }
    }


    public void SwitchTile(Tile clickedTile, Tile targetTile)
    {
        StartCoroutine(SwitchTileRoutine(clickedTile, targetTile));
    }

    private IEnumerator SwitchTileRoutine(Tile clickedTile, Tile targetTile)
    {
        if (_playerInputEnable && !GameManager.Instance.IsGameOver && !this.IsSwipping)
        {
            this.IsSwipping = true;
            GamePiece clickedPiece = this.AllGamePieces[clickedTile.xIndex, clickedTile.yIndex];
            GamePiece targetPiece = this.AllGamePieces[targetTile.xIndex, targetTile.yIndex];
            if (clickedPiece != null && targetPiece != null)
            {
                clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, SwapTime);
                targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, SwapTime);

                yield return new WaitForSeconds(SwapTime);
                List<GamePiece> clickedPieceMatches = this.BoardMatcher.FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
                List<GamePiece> targetPieceMatches = this.BoardMatcher.FindMatchesAt(targetTile.xIndex, targetTile.yIndex);
                List<GamePiece> colorMatches = this.BoardBomber.ProcessColorBomb(clickedPiece, targetPiece);

                if (clickedPieceMatches.Count == 0 && targetPieceMatches.Count == 0 && colorMatches.Count == 0)
                {
                    clickedPiece.Move(clickedTile.xIndex, clickedTile.yIndex, SwapTime);
                    targetPiece.Move(targetTile.xIndex, targetTile.yIndex, SwapTime);
                    yield return new WaitForSeconds(SwapTime);
                    this.IsSwipping = false;
                }
                else
                {
                    yield return new WaitForSeconds(SwapTime);
                    this.BoardBomber.ProcessBombs(clickedTile, targetTile, clickedPieceMatches, targetPieceMatches);
                    List<GamePiece> combinedListPieceMatches = clickedPieceMatches.Union(targetPieceMatches).ToList().Union(colorMatches).ToList();
                    yield return StartCoroutine(this.ClearAndRefillBoardRoutine(combinedListPieceMatches));
                    this.IsSwipping = false;
                    if (GameManager.Instance)
                    {
                        GameManager.Instance.UpdateMoveLeft();
                    }
                }
            }
        }

    }

    private void ClearAndRefillBoard(List<GamePiece> listgamePiece)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(listgamePiece));
    }

    public void ClearAndRefillBoard(int x, int y) //for zap booster
    {
        if (this.BoardQuery.IsWithinBounds(x, y))
        {
            GamePiece piece = this.AllGamePieces[x, y];

            List<GamePiece> listonePiece = new List<GamePiece>();
            listonePiece.Add(piece);
            this.ClearAndRefillBoard(listonePiece);
        }
    }

    public IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> listgamePiece)
    {
        this._playerInputEnable = false;
        this.IsRefilling = true;
        List<GamePiece> matches = listgamePiece;
        this.ScoreMultiplier = 0; //for continuous collapse
        do
        {
            this.ScoreMultiplier++;
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            yield return null;
            yield return StartCoroutine(this.BoardFiller.RefillRoutine());
            matches = this.BoardMatcher.FindAllMatches();
            yield return new WaitForSeconds(0.2f);
        }
        while (matches.Count != 0);

        if (this.BoardDeadLock.IsDeadLocked(this.AllGamePieces))
        {
            yield return new WaitForSeconds(1f);
            //this.ClearBoard();
            yield return StartCoroutine(this.BoardShuffler.ShuffleBoardRoutine(this));
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(this.BoardFiller.RefillRoutine());
        }

        this.IsRefilling = false;
        this._playerInputEnable = true;
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
            List<GamePiece> bombedPieces = this.BoardQuery.GetBombedPieces(listgamePiece);
            listgamePiece = listgamePiece.Union(bombedPieces).ToList();

            bombedPieces = this.BoardQuery.GetBombedPieces(listgamePiece);
            listgamePiece = listgamePiece.Union(bombedPieces).ToList();

            // start (for calculate CollectibleCount)
            List<GamePiece> collectedPieces = this.BoardQuery.GetCollectedPieces(listgamePiece);

            this.CollectibleCount -= collectedPieces.Count;
            // end ( for calculate CollectibleCount)
            listgamePiece = listgamePiece.Union(collectedPieces).ToList();
            List<int> columnsToCollapse = this.BoardQuery.GetColumns(listgamePiece);


            this.BoardClearer.ClearPieceAt(listgamePiece, bombedPieces);
            this.BoardTiles.BreakTileAt(listgamePiece);

            this.BoardBomber.ActivateBombs();

            yield return new WaitForSeconds(0.25f);
            movingPieces = this.BoardCollapser.CollapseColumn(columnsToCollapse);
            while (!this.BoardQuery.IsCollapsed(movingPieces))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
            matchesPieces = this.BoardMatcher.FindMatchesAt(movingPieces);
            collectedPieces = this.BoardQuery.FindCollectiblesAt(0, true);
            matchesPieces = matchesPieces.Union(collectedPieces).ToList();

            if (matchesPieces.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {
                this.ScoreMultiplier++;
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

    public void CheckDeadLock()
    {
        this.BoardDeadLock.IsDeadLocked(AllGamePieces);
    }

    public void ShuffleBoard() // when click input only
    {
        if (this._playerInputEnable)
        {
            StartCoroutine(this.BoardShuffler.ShuffleBoardRoutine(this));
        }
    }
}
