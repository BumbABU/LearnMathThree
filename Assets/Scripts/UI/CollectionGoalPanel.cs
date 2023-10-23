using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectionGoalPanel : MonoBehaviour
{
    public CollectionGoal CollectionGoal;
    public TextMeshProUGUI NumberLeftText;
    public Image PrefabImage;

    public void Start()
    {
        this.SetUpPanel();
    }

    public void SetUpPanel()
    {
        if (this.CollectionGoal != null && this.NumberLeftText != null && this.PrefabImage != null)
        {
            SpriteRenderer prefabSprite = this.CollectionGoal.PrefabToCollect.GetComponent<SpriteRenderer>();
            if (prefabSprite != null)
            {
                this.PrefabImage.sprite = prefabSprite.sprite;
                this.PrefabImage.color = prefabSprite.color;
            }
            this.NumberLeftText.text = this.CollectionGoal.NumberToCollect.ToString();
        }
    }

    public void UpdatePanel()
    {
        if (this.CollectionGoal != null && this.NumberLeftText != null)
        {
            this.NumberLeftText.text = this.CollectionGoal.NumberToCollect.ToString();
        }
    }
}
