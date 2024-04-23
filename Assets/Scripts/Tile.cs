using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public enum TileType
{
    Normal,
    Obstacle,
    Breakable
}

public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    public TileType TileType;
    private Board _board;
    private SpriteRenderer _sprite;

    public int BreakableValue = 0;
    public Sprite[] BreakableSprites;
    public Color NormalColor;
    void Awake()
    {
        this._sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        this.TouchInput();
    }

    public void Init(int x, int y, Board board)
    {
        this.xIndex = x;
        this.yIndex = y;
        this._board = board;
        if (TileType == TileType.Breakable)
        {
            if (this.BreakableSprites[BreakableValue] != null)
            {
                this._sprite.sprite = this.BreakableSprites[BreakableValue];
            }
        }
    }

    public void TouchInput()
    {
        if (Input.touchCount != 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 point = Camera.main.ScreenToWorldPoint(touch.position);
            Debug.Log("RUN1");
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
                    Debug.Log("RUN2");
                    if (hit && hit.collider.gameObject.CompareTag("Tile"))
                    {
                        Debug.Log("RUN3");
                        if (this._board != null)
                        {
                            Debug.Log("RUN4");
                            this._board.BoardInput.ClickTile(hit.collider.gameObject.GetComponent<Tile>());
                        }
                    }
                    break;
                case TouchPhase.Moved:
                    RaycastHit2D hitDrag = Physics2D.Raycast(point, Vector2.zero);
                    if (hitDrag && hitDrag.collider.gameObject.CompareTag("Tile"))
                    {
                        if (this._board != null)
                        {
                            this._board.BoardInput.DragToTile(hitDrag.collider.gameObject.GetComponent<Tile>());
                        }
                    }
                    break;
                case TouchPhase.Ended:
                    if (this._board != null)
                    {
                        this._board.BoardInput.ReleaseTile();
                    }
                    break;
            }
        }
    }

/*    public void OnMouseDown()
    {
        if (this._board != null)
        {
            this._board.BoardInput.ClickTile(this);
        }
    }

    public void OnMouseEnter()
    {
        if (this._board != null)
        {
            this._board.BoardInput.DragToTile(this);
        }
    }

    public void OnMouseUp()
    {
        if (this._board != null)
        {
            this._board.BoardInput.ReleaseTile();
        }
    }*/

    public void BreakTile()
    {
        if (this.TileType != TileType.Breakable)
        {
            return;
        }
        StartCoroutine(BreakTileRoutine());
    }

    private IEnumerator BreakTileRoutine()
    {
        BreakableValue = Mathf.Clamp(--BreakableValue, 0, BreakableValue);
        yield return new WaitForSeconds(0.25f);
        if (this.BreakableSprites[BreakableValue] != null)
        {
            this._sprite.sprite = this.BreakableSprites[BreakableValue];
        }

        if (BreakableValue == 0)
        {
            this.TileType = TileType.Normal;
            this._sprite.color = NormalColor;
        }
    }
}
