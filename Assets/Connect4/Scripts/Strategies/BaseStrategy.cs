using System;
using System.Collections.Generic;
using System.Linq;
using Connect4.Scripts.Commands;
using UnityEngine;

namespace Connect4.Scripts.Strategies
{
    public class BaseStrategy
    {
        private readonly IGridService _gridService;

        public List<BannedCommand> BannedCommands = new List<BannedCommand>();

        protected BaseStrategy(IGridService gridService)
        {
            _gridService = gridService;
        }

        protected bool GetOpponentStreak(ICommand command, Cell[] row, out List<Vector2Int> toBlock)
        {
            int maxIndex = _gridService.Width - 4;
            toBlock = new List<Vector2Int>();

            PlayerId playerId = command.ActivePlayer.PlayerId;

            for (var i = 0; i <= maxIndex; i++)
                if (ThreeInRaw(row, i, out Vector2Int index))
                    toBlock.Add(index);

            return toBlock.Count > 0;

            bool ThreeInRaw(Cell[] cells, int y, out Vector2Int index)
            {
                index = new Vector2Int();

                int counter = 0;
                bool hasEmptyCell = false;

                // Debug.Log("\n");

                // Debug.Log($"cell: {y}");
                // Debug.Log($"max: {Mathf.Clamp(y + 4, 0, cells.Length - 1)}");
                // Debug.Log($"Length: {cells.Length}");

                var maxLenght = Mathf.Clamp(y + 4, 0, cells.Length);
                Cell emptyCell = new Cell();

                for (int i = y; i < maxLenght; i++) //
                {
                    var cell = cells[i];

                    // Debug.Log($"index: {cell.Index} id: {cell.CellId}");

                    if (cell.CellId == playerId)
                        ++counter;

                    else if (cell.CellId == PlayerId.Empty)
                    {
                        if (_gridService.HasEmptyUnder(cell))
                        {
                            emptyCell.Initialize(cell.Index, cell.Position, cell.CellId);
                            continue;
                        }

                        hasEmptyCell = true;
                        index = cell.Index;
                    }
                }



                if (counter == 3 && emptyCell.Initialized)
                {
                    // Debug.Log($"added to ban: {new Vector2Int(emptyCell.Index.x + 1, emptyCell.Index.y)}");
                    emptyCell.Initialize(new Vector2Int(emptyCell.Index.x + 1, emptyCell.Index.y), emptyCell.Position, emptyCell.CellId);
                    
                    bool hasCommand = BannedCommands.Any(command => command.Index == emptyCell.Index);
                    
                    if (!hasCommand)
                    {
                        BannedCommand bannedCommand = new BannedCommand(emptyCell.Index, command.ActivePlayer);
                        BannedCommands.Add(bannedCommand);
                    }
                }

                return counter == 3 && hasEmptyCell;
            }
        }
    }
}