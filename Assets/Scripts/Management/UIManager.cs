using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class UIManager : Singleton<UIManager>
{
    public GameObject CollectionGoalLayout;
    public int CollectionGoalBaseWidth = 125;
    private CollectionGoalPanel[] _collectionGoalPanels;

    public ScreenFader ScreenFader;
    public TextMeshProUGUI LevelNameText;
    public TextMeshProUGUI MoveLeftText;
    public ScoreMeter ScoreMeter;
    public MessageWindow _messageWindow;
    public HighScoreWindow HighScoreWindow;
    public SettingWindow SettingWindow;

    public GameObject MovesCounter;
    public WinAllLevel WinAllLevelWindow;
    public Timer Timer;

    public bool IsSettingWindow;
    public bool IsHighScoreWindow = false;
    public bool IsCanTouch = true;

    public override void Awake()
    {
        base.Awake();
        if (this.ScreenFader != null)
        {
            this.ScreenFader.gameObject.SetActive(true);
        }
        if (this._messageWindow != null)
        {
            this._messageWindow.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
    }

    public void SetupCollectionGoalLayout(CollectionGoal[] collectionGoals, GameObject goalLayout, int spacingWidth)
    {
        if (goalLayout != null && collectionGoals != null && collectionGoals.Length > 0)
        {
            RectTransform rectxForm = goalLayout.GetComponent<RectTransform>();
            rectxForm.sizeDelta = new Vector2(spacingWidth * collectionGoals.Length, rectxForm.sizeDelta.y);
            CollectionGoalPanel[] panels = goalLayout.gameObject.GetComponentsInChildren<CollectionGoalPanel>();

            for (int i = 0; i < panels.Length; i++)
            {
                if (i < collectionGoals.Length && collectionGoals[i] != null)
                {
                    panels[i].CollectionGoal = collectionGoals[i];
                    panels[i].gameObject.SetActive(true);
                    panels[i].SetUpPanel();
                }
                else
                {
                    panels[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetupCollectionGoalLayout(CollectionGoal[] collectionGoals)
    {
        this.SetupCollectionGoalLayout(collectionGoals, this.CollectionGoalLayout, this.CollectionGoalBaseWidth);
    }

    public void UpdateCollectionGoalLayout(GameObject goalLayout)
    {
        if (goalLayout != null)
        {
            CollectionGoalPanel[] panels = goalLayout.gameObject.GetComponentsInChildren<CollectionGoalPanel>();
            foreach (CollectionGoalPanel panel in panels)
            {
                if (panel != null && panel.gameObject.activeInHierarchy)
                {
                    panel.UpdatePanel();
                }
            }
        }
    }

    public void UpdateCollectionGoalLayout()
    {
        this.UpdateCollectionGoalLayout(this.CollectionGoalLayout);
    }

    public void EnableTimer(bool state)
    {
        if (this.Timer != null)
        {
            this.Timer.gameObject.SetActive(state);
        }
    }

    public void EnableMovesCounter(bool state)
    {
        if (this.MovesCounter != null)
        {
            this.MovesCounter.gameObject.SetActive(state);
        }
    }

    public void EnableCollectionGoalLayout(bool state)
    {
        if (this.CollectionGoalLayout != null)
        {
            this.CollectionGoalLayout.gameObject.SetActive(state);
        }
    }



    public bool IsCanShowHighScoreWindow()
    {
        if (this.SettingWindow.gameObject.activeInHierarchy == true)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool IsCanShowSettingWindow()
    {
        if (this.HighScoreWindow.gameObject.activeInHierarchy == true)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ShowHighScoreWindow()
    {
        if (this.IsCanShowHighScoreWindow())
        {
            StartCoroutine(HighScoreWindow.ShowHighScoreWindowRoutine());
        }
    }

    public void ShowSettingWindow()
    {
        if(this.IsCanShowSettingWindow())
        {
            StartCoroutine(SettingWindow.ShowSettingWindowRoutine());
        }
    }

    public void ShowWinAllLevelWindow(float timeScale = 0.2f)
    {
        StartCoroutine(WinAllLevelWindow.ShowWinAllLevelWindow(timeScale));
    }
}
