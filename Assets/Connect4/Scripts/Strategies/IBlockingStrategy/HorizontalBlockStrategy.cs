using System.Collections.Generic;
using UnityEngine;

namespace Connect4.Scripts.Strategies.IBlockingStrategy
{
    public class HorizontalBlockStrategy : BaseStrategy, IBlockingStrategy
    {
        private readonly IGridService _gridService;

        public HorizontalBlockStrategy(IGridService gridService) : base(gridService)
        {
            _gridService = gridService;
        }
    
        public List<Vector2Int> GetIndexesToBlock(ICommand command)
        {
            return GetHorizontalToBlock(command);
        }
    
        private List<Vector2Int> GetHorizontalToBlock(ICommand command)
        {
            List<Vector2Int> toBlock = new List<Vector2Int>();
            int rowIndex = command.Index.x;

            if (HasStreak(command, rowIndex, out var list))
                toBlock.AddRange(list);

            if (rowIndex > 0 && HasStreak(command, rowIndex - 1, out list))
                toBlock.AddRange(list);

            return toBlock;
        
            bool HasStreak(ICommand command, int row, out List<Vector2Int> list)
            {
                list = new List<Vector2Int>();
            
                if(!_gridService.RawHasFreeCell(_gridService.GetRow(row))) 
                    return false;

                if (GetOpponentStreak(command, _gridService.GetRow(row), out var list2))
                {
                    list = list2;
                    return true;   
                }

                return false;
            }
        }
    }
}
