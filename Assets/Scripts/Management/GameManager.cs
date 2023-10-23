﻿using System.Collections;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelGoal))]
public class GameManager : Singleton<GameManager>
{
    private bool _isReadyToBegin = false;
    private bool _isReadyToChange = false;
    private bool _isGameOver = false;
    public bool IsGameOver
    {
        get { return this._isGameOver; }
    }
    private bool _isWinner = false;

    private Board _board;
    private LevelGoal _levelGoal;
    public LevelGoal LevelGoal
    {
        get { return _levelGoal; }
    }
    private LevelGoalCollected _levelGoalCollected;




    public override void Awake()
    {
        base.Awake();
        this._board = GameObject.FindObjectOfType<Board>();
        this._levelGoal = GetComponent<LevelGoal>();
        this._levelGoalCollected = GetComponent<LevelGoalCollected>();
    }

    public void Start()
    {
        if (UIManager.Instance)
        {
            if (UIManager.Instance.ScoreMeter != null)
            {
                UIManager.Instance.ScoreMeter.SetupStars(this._levelGoal);
            }

            if (UIManager.Instance.LevelNameText != null)
            {
                Scene scene = SceneManager.GetActiveScene();
                UIManager.Instance.LevelNameText.text = scene.name;
            }
            if (this._levelGoalCollected != null)
            {
                UIManager.Instance.EnableCollectionGoalLayout(true);
                UIManager.Instance.SetupCollectionGoalLayout(this._levelGoalCollected.CollectionGoals);
            }
            else
            {
                UIManager.Instance.EnableCollectionGoalLayout(false);
            }

            bool useTimer = (this._levelGoal.LevelCounter == LevelCounter.Timer);
            UIManager.Instance.EnableTimer(useTimer);
            UIManager.Instance.EnableMovesCounter(!useTimer);
        }


        this._levelGoal.MoveLeft++;
        this.UpdateMoveLeft();
        StartCoroutine("ExecuteGameLoop");
    }

    public void UpdateMoveLeft()
    {
        if (this._levelGoal.LevelCounter == LevelCounter.Moves)
        {
            this._levelGoal.MoveLeft--;
            if (UIManager.Instance != null && UIManager.Instance.MoveLeftText != null)
            {
                UIManager.Instance.MoveLeftText.text = this._levelGoal.MoveLeft.ToString();
            }
        }
        /*        else
                {
                    if (UIManager.Instance != null && UIManager.Instance.MoveLeftText != null)
                    {
                        UIManager.Instance.MoveLeftText.text = "\u221E";
                        UIManager.Instance.MoveLeftText.fontSize = 50;
                    }
                }*/
    }

    public void BeginGame()
    {
        this._isReadyToBegin = true;
    }

    private IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine("StartGameRoutine");
        yield return StartCoroutine("PlayGameRoutine");

        yield return StartCoroutine("WaitForBoardRoutine", 0f);
        yield return StartCoroutine("EndGameRoutine");
    }

    private IEnumerator WaitForBoardRoutine(float delay = 0f)
    {
        if (this._levelGoal.LevelCounter == LevelCounter.Timer && UIManager.Instance != null && UIManager.Instance.Timer != null)
        {
            UIManager.Instance.Timer.FadeOff();
            UIManager.Instance.Timer.Pause = true;
        }
        if (this._board != null)
        {
            yield return new WaitForSeconds(this._board.SwapTime); // why add swaptime here , check Board -> SwitchTileRoutine
            while (this._board.IsRefilling) // có liên quan tới IsRefilling nên ms thêm Swaptime
            {
                yield return null;
            }
        }
        yield return new WaitForSeconds(delay);
    }

    private IEnumerator StartGameRoutine()
    {
        if (UIManager.Instance)
        {
            if (UIManager.Instance._messageWindow != null)
            {
                UIManager.Instance._messageWindow.GetComponent<RectXformMover>().MoveOn();
                int maxScore = this.LevelGoal.ScoreGoals[this.LevelGoal.ScoreGoals.Length - 1];
                UIManager.Instance._messageWindow.ShowScoreMessage(maxScore);
                if (this._levelGoal.LevelCounter == LevelCounter.Timer)
                {
                    UIManager.Instance._messageWindow.ShowTimerGoal(this._levelGoal.TimeLeft);
                }
                else
                {
                    UIManager.Instance._messageWindow.ShowMovesGoal(this._levelGoal.MoveLeft);
                }

                if (this._levelGoalCollected != null)
                {
                    UIManager.Instance._messageWindow.ShowCollectionGoal(true);
                    GameObject goalLayout = UIManager.Instance._messageWindow.CollectionGoalLayout;
                    if (goalLayout != null)
                    {
                        UIManager.Instance.SetupCollectionGoalLayout(this._levelGoalCollected.CollectionGoals, goalLayout, 100);
                    }
                }
            }
        }

        while (!this._isReadyToBegin)
        {
            yield return null;
        }
        if (UIManager.Instance != null && UIManager.Instance.ScreenFader != null)
        {
            UIManager.Instance.ScreenFader.FadeOff();
        }
        yield return new WaitForSeconds(0.5f);
        if (this._board != null)
        {
            this._board.SetUpBoard();
        }
    }

    private IEnumerator PlayGameRoutine()
    {
        if (this._levelGoal.LevelCounter == LevelCounter.Timer)
        {
            this._levelGoal.StartCountdown();
        }
        AudioSource music = null;
        while (!this._isGameOver)
        {
            this._isGameOver = this._levelGoal.IsGameOver();
            this._isWinner = this._levelGoal.IsWinner();
            if(SoundManager.Instance)
            {
                if(music == null)
                {
                    Debug.Log("ee");
                    music = SoundManager.Instance.PlayClipAtPoint(SoundManager.Instance.MusicClips[1], Vector3.zero);
                    music.Stop();
                    music.Play();
                }
            }
            yield return null;
        }
    }

    private IEnumerator EndGameRoutine()
    {
        this._isReadyToChange = false;
        if (this._isWinner)
        {
            this.ShowWinScreen();
        }
        else
        {
            this.ShowLoseScreen();
        }
        yield return new WaitForSeconds(1);
        if (UIManager.Instance != null && UIManager.Instance.ScreenFader != null)
        {
            UIManager.Instance.ScreenFader.FadeOn();
        }
        while (!this._isReadyToChange)
        {
            yield return null;
        }
        /*        SceneManager.LoadScene(SceneManager.GetActiveScene().name);*/
      SceneManager.LoadScene("Moves");
     //Application.Quit();
    }

    private void ShowLoseScreen()
    {
        if (UIManager.Instance != null && UIManager.Instance._messageWindow != null)
        {
            UIManager.Instance._messageWindow.GetComponent<RectXformMover>().MoveOn();
            UIManager.Instance._messageWindow.ShowLoseMessage();
            UIManager.Instance._messageWindow.ShowCollectionGoal(false);
            string caption = "";
            if (this._levelGoal.LevelCounter == LevelCounter.Timer)
            {
                caption = "OUT OF TIME!";
            }
            else
            {
                caption = "OUT OF MOVES!";
            }
            UIManager.Instance._messageWindow.ShowGoalCaption(caption, 0, 70);
            if (UIManager.Instance._messageWindow.GoalCompleteIcon != null)
            {
                UIManager.Instance._messageWindow.ShowGoalImage(UIManager.Instance._messageWindow.GoalFailedIcon);
            }
        }
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlayRandomLoseSound();
        }
    }

    private void ShowWinScreen()
    {
        if (UIManager.Instance != null && UIManager.Instance._messageWindow != null)
        {
            UIManager.Instance._messageWindow.GetComponent<RectXformMover>().MoveOn();
            UIManager.Instance._messageWindow.ShowWinMessage();
            UIManager.Instance._messageWindow.ShowCollectionGoal(false);
            if (UIManager.Instance._messageWindow.GoalCompleteIcon != null)
            {
                UIManager.Instance._messageWindow.ShowGoalImage(UIManager.Instance._messageWindow.GoalCompleteIcon);
            }
            if (ScoreManager.Instance)
            {
                string scoreStr = "YOU SCORED\n" + ScoreManager.Instance.CurrentScore.ToString() + " POINTS";
                UIManager.Instance._messageWindow.ShowGoalCaption(scoreStr, 0, 70);
            }
        }
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlayRandomWinSound();
        }
    }

    public void ReloadScene()
    {
        this._isReadyToChange = true;
    }

    public void ScorePoints(GamePiece gamePiece, int multiplier = 1, int bonus = 0)
    {
        if (gamePiece != null)
        {
            if (ScoreManager.Instance)
            {
                ScoreManager.Instance.AddScore(gamePiece.ScoreValue * multiplier + bonus);
                this._levelGoal.UpdateSocreStar(ScoreManager.Instance.CurrentScore);
                if (UIManager.Instance != null && UIManager.Instance.ScoreMeter != null)
                {
                    UIManager.Instance.ScoreMeter.UpdateScoreMetter(ScoreManager.Instance.CurrentScore, this._levelGoal.ScoreStar);
                }
            }

            if (SoundManager.Instance && gamePiece.ClearSound != null)
            {
                SoundManager.Instance.PlayClipAtPoint(gamePiece.ClearSound, Vector3.zero, SoundManager.Instance.FxVolume);
            }
        }
    }

    public void AddTime(int timeValue)
    {
        if (this._levelGoal.LevelCounter == LevelCounter.Timer)
        {
            this._levelGoal.AddTime(timeValue);
        }
    }

    public void UpdateCollectionsGoals(GamePiece pieceToCheck)
    {
        if (pieceToCheck != null)
        {
            this._levelGoalCollected.UpdateCollectedGoals(pieceToCheck);
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void NextLevel ()
    {

    }
}