using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class LevelGoalCollected : LevelGoal
{
    public CollectionGoal[] CollectionGoals;

    public void UpdateCollectedGoals(GamePiece pieceToCheck)
    {
        if (pieceToCheck != null)
        {
            foreach (CollectionGoal go in CollectionGoals)
            {
                if (go != null)
                {
                    go.CollectedPiece(pieceToCheck);
                }
            }
        }
        this.UpdateUI();
    }

    public void UpdateUI()
    {
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateCollectionGoalLayout();
        }
    }

    private bool AreGoalsComplete(CollectionGoal[] goals)
    {
        foreach (CollectionGoal go in goals)
        {
            if (go == null || goals == null)
            {
                return false;
            }
            if (goals.Length == 0)
            {
                return false;
            }
            if (go.NumberToCollect != 0)
            {
                return false;
            }
        }
        return true;
    }

    public override bool IsGameOver()
    {
        if (this.AreGoalsComplete(CollectionGoals) && ScoreManager.Instance)
        {
            int maxScore = this.ScoreGoals[this.ScoreGoals.Length - 1];
            if (ScoreManager.Instance.CurrentScore >= maxScore)
            {
                return true;
            }
        }
        if (this.LevelCounter == LevelCounter.Moves)
        {
            return (this.MoveLeft <= 0);
        }
        else
        {
            return(this.TimeLeft <= 0);
        }
    }

    public override bool IsWinner()
    {
        if (ScoreManager.Instance != null)
        {
            return (ScoreManager.Instance.CurrentScore >= this.ScoreGoals[0] && this.AreGoalsComplete(this.CollectionGoals));
        }
        return false;
    }
}
