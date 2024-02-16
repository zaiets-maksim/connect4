using System.Collections.Generic;
using UnityEngine;

namespace Connect4.Scripts.Strategies
{
    public class BaseStrategy
    {
        protected IGridService _gridService;
        protected IGameCurator _gameCurator;

        protected BaseStrategy(IGridService gridService)
        {
            _gridService = gridService;
        }

        protected bool GetOpponentStreak(ICommand command, Cell[] row, out List<Vector2Int> indexes, int streak)
        {
            int maxIndex = _gridService.Width - 4;
            indexes = new List<Vector2Int>();

            PlayerId playerId = command.ActivePlayer.PlayerId;

            for (var i = 0; i <= maxIndex; i++)
                if (IsStreak(row, i, out Vector2Int index))
                    indexes.Add(index);

            return indexes.Count > 0;

            bool IsStreak(Cell[] cells, int y, out Vector2Int index)
            {
                index = new Vector2Int();

                int counter = 0;
                bool hasEmptyCell = false;

                // Debug.Log("\n");

                var maxLenght = Mathf.Clamp(y + 4, 0, cells.Length);


                for (int i = y; i < maxLenght; i++) //
                {
                    var cell = cells[i];

                    // Debug.Log($"index: {cell.Index} id: {cell.CellId}");
                    
                    if (cell.CellId == playerId)
                        ++counter;
                    
                    else if (cell.CellId == PlayerId.Empty)
                    {
                        if (_gridService.HasEmptyUnder(cell))
                            continue;

                        hasEmptyCell = true;
                        index = cell.Index;
                    }
                    
                    else
                        return false;
                }

                // Debug.Log($"streak: {counter}");
                // Debug.Log(counter == streak && hasEmptyCell);
                // Debug.Log("\n");

                return counter == streak && hasEmptyCell;
            }
        }
    }
}