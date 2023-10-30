using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class Booster : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image _image;
    private RectTransform _rectXForm;
    private Vector3 _startPosition;
    private Board _board;
    private Tile _tile;

    public static GameObject ActiveBooster;
    public TextMeshProUGUI InstructionsText;
    public TextMeshProUGUI ZapBoosterText;
    public TextMeshProUGUI ColorBombBoosterText;
    public TextMeshProUGUI TimeBoosterText;
    public string Instructions = "drag over game piece to remove";

    public bool IsEnabled = false;
    public bool IsDraggable = true;
    public bool IsLocked = false;

    public List<CanvasGroup> CanvasGroup;
    public UnityEvent BoostEven;
    public int BoostTime = 15;


    private void Awake()
    {
        this._image = GetComponent<Image>();
        this._rectXForm = GetComponent<RectTransform>();
        this._board = Object.FindObjectOfType<Board>();
    }

    private void Start()
    {
        this.EnableBooster(false);
    }

    public void EnableBooster(bool state)
    {
        this.IsEnabled = state;
        if (state)
        {
            this.DisableOtherBooster();
            Booster.ActiveBooster = this.gameObject;
        }
        else if (this.gameObject == Booster.ActiveBooster)
        {
            Booster.ActiveBooster = null;
        }
        this._image.color = state ? Color.white : Color.gray;
        if (this.InstructionsText != null)
        {
            this.InstructionsText.gameObject.SetActive(Booster.ActiveBooster != null);
            if (gameObject == Booster.ActiveBooster)
            {
                this.InstructionsText.text = this.Instructions;
            }
        }
        this.UpdateColorBombBoosterText();
        this.UpdateTimeBoosterText();
        this.UpdateZapBoosterText();
    }

    public void DisableOtherBooster()
    {
        Booster[] allBooster = Object.FindObjectsOfType<Booster>();
        foreach (Booster booster in allBooster)
        {
            if (booster != this)
            {
                booster.EnableBooster(false);
            }
        }
    }

    public void ToggleBooster()
    {
        // this.EnableBooster(!IsEnabled);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //this.EnableBooster(true);
        this.IsDraggable = true;
        if (this.IsDraggable && this.IsEnabled && !this.IsLocked)
        {
            this._startPosition = this.gameObject.transform.position;
            this.EnableCanvasGroup(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (this.IsDraggable && this.IsEnabled && !this.IsLocked && Camera.main != null)
        {
            Vector3 screenPosition;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(this._rectXForm, eventData.position, Camera.main, out screenPosition);
            this.gameObject.transform.position = screenPosition;

            RaycastHit2D hit = Physics2D.Raycast(screenPosition, Vector3.forward, Mathf.Infinity);
            if (hit.collider != null)
            {
                this._tile = hit.collider.GetComponent<Tile>();
            }
            else
            {
                this._tile = null;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (this.IsDraggable && this.IsEnabled && !this.IsLocked)
        {
            this.gameObject.transform.position = this._startPosition;
            this.EnableCanvasGroup(true);
            if (this._board != null && !this._board.IsRefilling)
            {
                if (this.BoostEven != null)
                {
                    this.BoostEven.Invoke();
                }
                this.EnableBooster(false);
                this._tile = null;
                Booster.ActiveBooster = null;
            }
        }
    }

    public void EnableCanvasGroup(bool state)
    {
        if (this.CanvasGroup != null && this.CanvasGroup.Count > 0)
        {
            foreach (CanvasGroup cGroup in this.CanvasGroup)
            {
                cGroup.blocksRaycasts = state;
            }
        }
    }

    public void ReMoveOneGamepiece()
    {
        if (this._board != null && this._tile != null)
        {
            if (GameManager.Instance)
            {
                if (GameManager.Instance.ZapBooster <= 0) return;
                GameManager.Instance.ZapBooster--;
            }
            this._board.ClearAndRefillBoard(this._tile.xIndex, this._tile.yIndex);
        }
    }

    public void AddTime()
    {
        if (GameManager.Instance)
        {
            if (GameManager.Instance.TimeBooster <= 0) return;
            GameManager.Instance.TimeBooster--;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddTime(this.BoostTime);
        }
    }

    public void DropColorBomb()
    {
        if (this._board != null && this._tile != null)
        {
            if (GameManager.Instance)
            {
                if (GameManager.Instance.ColorBombBooster <= 0) return;
                GameManager.Instance.ColorBombBooster--;
            }
            this._board.BoardFiller.MakeColorBombBooster(this._tile.xIndex, this._tile.yIndex);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.EnableBooster(true);
        this.IsDraggable = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!this.IsDraggable)
        {
            this.EnableBooster(false);
        }
    }

    public void UpdateZapBoosterText ()
    {
        if(this.ZapBoosterText != null)
        {
            if(GameManager.Instance)
            {
                this.ZapBoosterText.text = GameManager.Instance.ZapBooster.ToString();
            }
        }
    }

    public void UpdateColorBombBoosterText()
    {
        if (this.ColorBombBoosterText != null)
        {
            if (GameManager.Instance)
            {
                this.ColorBombBoosterText.text = GameManager.Instance.ColorBombBooster.ToString();
            }
        }
    }

    public void UpdateTimeBoosterText()
    {
        if (this.TimeBoosterText != null)
        {
            if (GameManager.Instance)
            {
                this.TimeBoosterText.text = GameManager.Instance.TimeBooster.ToString();
            }
        }
    }
}
