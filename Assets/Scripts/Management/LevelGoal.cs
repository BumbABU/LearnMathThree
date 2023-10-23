using System.Collections;
using UnityEngine;

public enum LevelCounter
{
    Timer,
    Moves
}
public abstract class LevelGoal : Singleton<LevelGoal>
{
    public int ScoreStar = 0;
    public int[] ScoreGoals = new int[3] { 1000, 2000, 3000 };
    public int MoveLeft = 30;
    public int TimeLeft = 60;
    private int _maxTime;
    public LevelCounter LevelCounter = LevelCounter.Moves;

    public virtual void Start()
    {
        this.Init();
        if(this.LevelCounter == LevelCounter.Timer)
        {
            this._maxTime = this.TimeLeft;
            if (UIManager.Instance != null && UIManager.Instance.Timer != null)
            {
                UIManager.Instance.Timer.InitTimer(this.TimeLeft);
            }
        }
    }

    private void Init()
    {
        this.ScoreStar = 0;
        for (int i = 1; i < ScoreGoals.Length; i++)
        {
            if (this.ScoreGoals[i] < this.ScoreGoals[i - 1])
            {
                Debug.LogWarning("LEVELGOAL Setup score goals in increasing order ");
            }
        }
    }

    private int UpdateScore(int score)
    {
        for (int i = 0; i < this.ScoreGoals.Length; i++)
        {
            if (score < this.ScoreGoals[i])
            {
                return i;
            }
        }
        return ScoreGoals.Length;
    }

    public void UpdateSocreStar(int score)
    {
        this.ScoreStar = this.UpdateScore(score);
    }

    public abstract bool IsWinner();
    public abstract bool IsGameOver();

    public void StartCountdown()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        while (this.TimeLeft > 0)
        {
            yield return new WaitForSeconds(1);
            this.TimeLeft--;
            if (UIManager.Instance != null && UIManager.Instance.Timer != null)
            {
                UIManager.Instance.Timer.UpdateTimer(this.TimeLeft);
            }
        }
    }

    public void AddTime(int timeValue)
    {
        this.TimeLeft += timeValue;
        this.TimeLeft = Mathf.Clamp(this.TimeLeft, 0, this._maxTime);
        if (UIManager.Instance != null && UIManager.Instance.Timer != null)
        {
            UIManager.Instance.Timer.UpdateTimer(this.TimeLeft);
        }
    }
}
