using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalScored : LevelGoal
{
    public override void Start()
    {
        this.LevelCounter = LevelCounter.Moves;
        base.Start();
    }
    public override bool IsWinner()
    {
        if (ScoreManager.Instance != null)
        {
            return (ScoreManager.Instance.CurrentScore >= this.ScoreGoals[0]);
        }
        return false;
    }
    public override bool IsGameOver()
    {
        int maxScore = this.ScoreGoals[this.ScoreGoals.Length - 1];
        if(ScoreManager.Instance)
        {
            if(ScoreManager.Instance.CurrentScore >= maxScore)
            {
                return true;
            }
        }
        return (MoveLeft == 0);
    }

}
