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
        Debug.Log(this.gameObject.name);
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
            this._board.ClearAndRefillBoard(this._tile.xIndex, this._tile.yIndex);
        }
    }

    public void AddTime()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddTime(this.BoostTime);
        }
    }

    public void DropColorBomb()
    {
        if (this._board != null && this._tile != null)
        {
            this._board.MakeColorBombBooster(this._tile.xIndex, this._tile.yIndex);
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
}
