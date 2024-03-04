using System.Collections.Generic;
using Connect4.Scripts.Field;
using UnityEngine;

namespace Connect4.Scripts.Strategies.Win
{
    public interface IWinningStrategy
    {
        public List<Vector2Int> FourInLine { get; }
        bool IsWinningMove(Vector2Int index, PlayerId playerId);
    }
}
