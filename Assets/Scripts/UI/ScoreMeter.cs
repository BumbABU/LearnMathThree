using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ScoreMeter : MonoBehaviour
{
    public Slider Slider;
    public ScoreStar[] ScoreStars = new ScoreStar[3];
    private LevelGoal _levelGoal;
    private int maxScore;

    public void Awake()
    {
        this.Slider = GetComponent<Slider>();
    }

    public void SetupStars(LevelGoal levelGoal)
    {
        if (levelGoal == null)
        {
            Debug.LogWarning("SCOREMETER Invalid levelGoal");
            return;
        }
        this._levelGoal = levelGoal;
        maxScore = this._levelGoal.ScoreGoals[this._levelGoal.ScoreGoals.Length - 1];

        float sliderWidth = this.Slider.GetComponent<RectTransform>().rect.width;
        if (maxScore > 0)
        {
            for (int i = 0; i < this._levelGoal.ScoreGoals.Length; i++)
            {
                if (this.ScoreStars[i] != null)
                {
                    float newX = (sliderWidth * this._levelGoal.ScoreGoals[i] / maxScore) - (sliderWidth * 0.5f);// why - (sliderwith * 0.5f), because anchoredPosition of ScoreStar in the hafl of slider width 
                    RectTransform starRectXForm = this.ScoreStars[i].GetComponent<RectTransform>();
                    if (starRectXForm != null)
                    {
                        starRectXForm.anchoredPosition = new Vector2(newX, starRectXForm.anchoredPosition.y);
                    }
                }
            }
        }
    }

    public void UpdateScoreMetter(int score, int starCount)
    {
        if(this._levelGoal != null)
        {
            this.Slider.value = (float)score / (float)maxScore;
        }
        for (int i = 0; i < starCount; i++)
        {
            if (this.ScoreStars[i] != null)
            {
                this.ScoreStars[i].Activate();
            }
        }
    }
}
