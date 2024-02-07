using UnityEngine;

namespace Connect4.Scripts.IWinningStrategy
{
    public class VerticalWinStrategy : IWinningStrategy
    {
        public bool IsWinningMove(IGridService gridService, Vector2Int index, PlayerId playerId)
        {
            int height = gridService.Height;

            if (index.x > height - 4)
                return false;

            var column = gridService.GetColumn(index.y);
            var result = FourInColumn(index.x, column, playerId);

            if(result)
                Debug.Log($"<color=green>{playerId} </color> win!");
            
            return result;
        }
        
        private bool FourInColumn(int index, Cell[] column, PlayerId playerId)
        {
            for (var i = index; i < index + 4; i++)
                if (column[i].CellId != playerId)
                    return false;

            return true;
        }
    }
}
