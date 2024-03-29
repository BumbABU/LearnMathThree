using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(RectXformMover))]
public class MessageWindow : MonoBehaviour
{
    public Image MessageIcon;
    public TextMeshProUGUI MessageText;
    public TextMeshProUGUI ButtonText;

    public Sprite WinIcon;
    public Sprite LoseIcon;
    public Sprite GoalIcon;

    //

    public Sprite CollectIcon;
    public Sprite MovesIcon;
    public Sprite TimerIcon;
    public Sprite GoalCompleteIcon;
    public Sprite GoalFailedIcon;

    public Image GoalImage;
    public TextMeshProUGUI GoalText;
    public GameObject CollectionGoalLayout;

    public GameObject StarPanel;
    public GameObject GoodLuckPanel;
    public GameObject WinButtonPanel;
    public GameObject LoseButtonPanel;
    public GameObject StartButton;

    // Star Rating
    public StarRating StarRating;

    public void ShowMessage(Sprite sprite = null, string message = "", string buttonMessage = "START")
    {
        if (this.MessageIcon != null)
        {
            this.MessageIcon.sprite = sprite;
        }
        if (this.MessageText != null)
        {
            this.MessageText.text = message;
        }
        if (this.ButtonText != null)
        {
            this.ButtonText.text = buttonMessage;
        }
    }

    public void ShowScoreMessage(int scoreGoal)
    {
        string message = "SCORE GOAL \n" + scoreGoal.ToString();
        this.ShowMessage(this.GoalIcon, message, "START");
    }

    public void ShowWinMessage()
    {
        this.ShowMessage(this.WinIcon, "LEVEL\nCOMPLETE", "OK");
    }

    public void ShowLoseMessage()
    {
        this.ShowMessage(this.LoseIcon, "LEVEL\nFAILED", "OK");
    }

    public void ShowGoal(Sprite icon = null, string caption = "")
    {
        if (caption != "")
        {
            this.ShowGoalCaption(caption);
        }
        if (icon != null)
        {
            this.ShowGoalImage(icon);
        }
    }

    public void ShowMovesGoal(int movesLeft)
    {
        string caption = movesLeft.ToString() + " MOVES";
        this.ShowGoal(MovesIcon, caption);
    }

    public void ShowTimerGoal(int timeLeft)
    {
        string caption = timeLeft.ToString() + " SECONDS";
        this.ShowGoal(TimerIcon, caption);
    }

    public void ShowCollectionGoal(bool state = true)
    {
        if (CollectionGoalLayout != null)
        {
            this.CollectionGoalLayout.gameObject.SetActive(state);
        }
        if (state)
        {
            this.ShowGoal(CollectIcon, "");
        }
    }

    public void ShowGoalCaption(string caption = "", int xOffset = 0, int yOffset = 0)
    {
        if (this.GoalText != null)
        {
            this.GoalText.text = caption;
            RectTransform rectXform = this.GoalText.GetComponent<RectTransform>();
            rectXform.anchoredPosition += new Vector2(xOffset, yOffset);
        }
    }

    public void ShowGoalImage(Sprite icon = null)
    {
        if (this.GoalImage != null && icon != null)
        {
            this.GoalImage.gameObject.SetActive(true);
            this.GoalImage.sprite = icon;
        }
        else
        {
            this.GoalImage.gameObject.SetActive(false);
        }
    }

    public void ShowButtonPanel(bool startButton = false, bool winpanel = false)
    {
        if (startButton)
        {
            this.StartButton.gameObject.SetActive(true);
            this.WinButtonPanel.gameObject.SetActive(false);
            this.LoseButtonPanel.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.StartButton.gameObject.SetActive(false);
            if (winpanel)
            {
                this.WinButtonPanel.gameObject.SetActive(true);
                this.LoseButtonPanel.gameObject.SetActive(false);
            }
            else
            {
                this.WinButtonPanel.gameObject.SetActive(false);
                this.LoseButtonPanel.gameObject.SetActive(true);
            }
        }

    }

    public void ShowGoodLuckPanel(bool state)
    {
        if (this.GoodLuckPanel != null)
        {
            this.GoodLuckPanel.gameObject.SetActive(state);
        }
    }

    public void ShowStarPanel(bool state)
    {
        if(this.StarPanel != null)
        {
            this.StarPanel.gameObject.SetActive(state);
        }
    }

    public void RateStar(int numStar, float timeScale = 0.2f)
    {
        if(this.StarRating != null)
        {
            StartCoroutine(StarRating.RatingStarRoutine(numStar, timeScale));
        }
    }
}
