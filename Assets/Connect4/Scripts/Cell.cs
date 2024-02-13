using System;
using UnityEngine;

[Serializable]
public class Cell
{
    public Vector2Int Index { get; private set; }
    public Vector2 Position { get; private set; }
    public PlayerId CellId { get; private set; } = PlayerId.Empty;
    public bool Initialized { get; private set; }

    public void Initialize(Vector2Int index, Vector2 position, PlayerId cellId = PlayerId.Empty)
    {
        Index = index;
        Position = position;
        CellId = cellId;
        Initialized = true;
    }

    public void Take(PlayerId cellId) => 
        CellId = cellId;

    public void Release() => 
        CellId = PlayerId.Empty;
}

public enum PlayerId
{
    Empty,
    Player1,
    Player2
}
