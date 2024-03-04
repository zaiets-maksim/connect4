using System.Collections.Generic;
using Connect4.Scripts.Commands;
using Connect4.Scripts.Field;
using Connect4.Scripts.Services.GridService;
using UnityEngine;

namespace Connect4.Scripts.Strategies
{
    public class BaseStrategy
    {
        protected const int WinStreak = 4;
        protected IGridService _gridService;

        protected BaseStrategy(IGridService gridService)
        {
            _gridService = gridService;
        }

        protected bool GetOpponentStreak(ICommand command, Cell[] cells, out List<Vector2Int> indexes, int streak)
        {
            int maxIndex = _gridService.Width - WinStreak;
            indexes = new List<Vector2Int>();

            PlayerId playerId = command.ActivePlayer.PlayerId;

            for (var i = 0; i <= maxIndex; i++)
            {
                if (IsStreak(cells, i, out Vector2Int index)) 
                    indexes.Add(index);
            }

            return indexes.Count > 0;

            bool IsStreak(Cell[] cells, int y, out Vector2Int index)
            {
                index = new Vector2Int();

                int counter = 0;
                bool hasEmptyCell = false;
                
                var maxLenght = Mathf.Clamp(y + WinStreak, 0, cells.Length);

                for (int i = y; i < maxLenght; i++)
                {
                    var cell = cells[i];
                    // Debug.Log($"index: {cell.Index} id: {cell.CellId}");

                    if (cell.CellId != playerId && cell.CellId != PlayerId.Empty)
                        return false;
                    
                    if (cell.CellId == playerId)
                        ++counter;

                    else if (cell.CellId == PlayerId.Empty)
                    {
                        if (_gridService.HasEmptyUnder(cell))
                            continue;

                        hasEmptyCell = true;

                        
                        if (cells.Length < 4)
                            return false;

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

        protected List<Cell> GetDiagonal(Vector2Int index, int stepJ)
        {
            var diagonal = new List<Cell>();

            for (int i = index.x, j = index.y; IndexInOfRange(i, j); i--, j += stepJ)
                diagonal.Add(_gridService.Grid[i][j]);

            return diagonal;
        }

        protected bool IndexInOfRange(int i, int j) =>
            i >= 0 && j >= 0 && i < _gridService.Height && j < _gridService.Width;
    }
}