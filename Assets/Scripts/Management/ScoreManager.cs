using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : Singleton<ScoreManager>
{
    private int _currentScore = 0; // hiển thị số cuối cùng sau khi countervalue = currentscore
    public int CurrentScore
    {
        get { return _currentScore; }
    }
    private int _counterValue = 0; // hiển thị số chạy động
    [SerializeField] private int _increment = 5;

    public TextMeshProUGUI ScoreText;
    void Start()
    {
        this.UpdateScoreText(this._currentScore);
    }


    public void UpdateScoreText(int scoreValue)
    {
        if (this.ScoreText != null)
        {
            this.ScoreText.text = scoreValue.ToString();
        }
    }

    public void AddScore(int value)
    {
        this._currentScore += value;
        StartCoroutine(this.CountScoreRoutine());
    }

    private IEnumerator CountScoreRoutine()
    {
        int interations = 0;
        while (this._counterValue < this._currentScore && interations < 100000)
        {
            this._counterValue += this._increment;
            this._counterValue = Mathf.Clamp(this._counterValue, 0, this._currentScore);
            this.UpdateScoreText(this._counterValue);
            interations++;
            yield return null;
        }
        this._currentScore = this._counterValue;
        this.UpdateScoreText(this._currentScore);
    }

    public void SetHighSocre(int score)
    {
        int oldHighScore = 0;
        if (PlayerPrefs.HasKey("HiScore"))
        {
            oldHighScore = PlayerPrefs.GetInt("HiScore");
        }

        if (score > oldHighScore)
        {
            PlayerPrefs.SetInt("HiScore", score);
        }
    }

    public string GetHighScore()
    {
        int highScore = 0;
        if (PlayerPrefs.HasKey("HiScore"))
        {
            highScore = PlayerPrefs.GetInt("HiScore", highScore);
        }
        return highScore.ToString();
    }

    public void SetHighRateStarLevel(int rateStar)
    {
        int oldHighStar = 0;
        int currenLevelidx = SceneManager.GetActiveScene().buildIndex;
        string levelName = "Level" + currenLevelidx;
        if (PlayerPrefs.HasKey(levelName))
        {
            oldHighStar = PlayerPrefs.GetInt(levelName);
        }

        if (rateStar > oldHighStar)
        {
            PlayerPrefs.SetInt(levelName, rateStar);
        }
    }


    public int GetHighRateStar()
    {
        int rateStar = 0;
        int currenLevelidx = SceneManager.GetActiveScene().buildIndex;
        string levelName = "Level" + currenLevelidx;
        if (PlayerPrefs.HasKey(levelName))
        {
            rateStar = PlayerPrefs.GetInt(levelName);
        }
        return rateStar;
    }

/*    public void ResetHighRate()
    {
        int currenLevelidx = SceneManager.GetActiveScene().buildIndex;
        string levelName = "Level" + currenLevelidx;
        PlayerPrefs.SetInt(levelName, 1);
    }*/

}
