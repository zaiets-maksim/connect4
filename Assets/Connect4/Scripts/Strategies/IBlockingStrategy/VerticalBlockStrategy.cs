using System.Collections.Generic;
using UnityEngine;

namespace Connect4.Scripts.Strategies.IBlockingStrategy
{
    public class VerticalBlockStrategy : BaseStrategy, IBlockingStrategy
    {
        private readonly IGridService _gridService;

        public VerticalBlockStrategy(IGridService gridService) : base(gridService)
        {
            _gridService = gridService;
        }
        
        public List<Vector2Int> GetIndexesToBlock(ICommand command)
        {
            return HasColumnToBlock(command);
        }
        
        private List<Vector2Int> HasColumnToBlock(ICommand command)
        {
            List<Vector2Int> indexes = new List<Vector2Int>();
        
            if (_gridService.Columns[command.Index.y].HasFreeCell)
            {
                var verticalStreak = GetOpponentStreak
                    (command.Index,
                    _gridService.GetColumn(command.Index.y),
                    _gridService.Height, command.ActivePlayer.PlayerId);

                if (verticalStreak > 2)
                {
                    indexes.Add(new Vector2Int(command.Index.x + 1, command.Index.y));
                    Debug.Log($"verticalsStreak is {verticalStreak}");
                }
            }

            return indexes;
        }
        
        private int GetOpponentStreak(Vector2Int index, Cell[] column, int height, PlayerId playerId)
        {
            int counter = 0;

            for (int x = index.x; x < height; x++)
            {
                // Debug.Log(column[x].Index);
                if (column[x].CellId == playerId)
                    ++counter;
            }

            return counter;
        }
    }
}
