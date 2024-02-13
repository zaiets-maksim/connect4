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
            var result = FourInColumn(column, index.x, playerId);

            // if(result)
            //     Debug.Log($"<color=green>{playerId} </color> win!");
            
            return result;
        }
        
        private bool FourInColumn(Cell[] column, int index, PlayerId playerId)
        {
            // Debug.Log("\n");
            for (var i = index; i < index + 4; i++)
            {
                // Debug.Log($"index: {column[i].Index} id: {column[i].CellId}");
                if (column[i].CellId != playerId)
                    return false;
            }

            return true;
        }
    }
}
