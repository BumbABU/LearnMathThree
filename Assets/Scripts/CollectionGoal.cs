using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionGoal : MonoBehaviour
{
    public GamePiece PrefabToCollect;
    [Range(1, 50)]
    public int NumberToCollect = 5;

    private SpriteRenderer _spriteRenderer;

    public void Start()
    {
        if(PrefabToCollect != null)
        {
            this._spriteRenderer = PrefabToCollect.GetComponent<SpriteRenderer>();
        }
    }

    public void CollectedPiece(GamePiece piece)
    {
        if(piece != null)
        {
            SpriteRenderer spriteRenderer = piece.GetComponent<SpriteRenderer>();
            if(this._spriteRenderer.sprite == spriteRenderer.sprite && this.PrefabToCollect.MatchValue == piece.MatchValue)
            {
                this.NumberToCollect--;
                this.NumberToCollect = Mathf.Clamp(this.NumberToCollect, 0, this.NumberToCollect);
            }
        }
    }
}
