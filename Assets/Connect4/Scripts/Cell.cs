using System;
using UnityEngine;

[Serializable]
public class Cell
{
    public Vector2Int Index { get; private set; }
    public Vector2 Position { get; private set; }
    public PlayerId CellId { get; private set; }

    public void Initialize(Vector2Int index, Vector2 position)
    {
        Index = index;
        Position = position;
    }

    public void Take(PlayerId state) => 
        CellId = state;
}

public enum PlayerId
{
    Empty,
    Player1,
    Player2
}
