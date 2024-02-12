using UnityEngine;

namespace Connect4.Scripts.IWinningStrategy
{
    public interface IWinningStrategy
    {
        bool IsWinningMove(IGridService gridService, Vector2Int index, PlayerId playerId);
    }
}
