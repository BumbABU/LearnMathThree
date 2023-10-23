using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : GamePiece
{
    public bool ClearedByBomb = false;
    public bool ClearedAtBottom = false;

    void Start()
    {
        this.MatchValue = MatchValue.None;
    }
}
