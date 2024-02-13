using UnityEngine;

namespace Connect4.Scripts.IWinningStrategy
{
    public class HorizontalWinStrategy : IWinningStrategy
    {
        public bool IsWinningMove(IGridService gridService, Vector2Int index, PlayerId playerId)
        {
            int width = gridService.Width;
            var row = gridService.GetRow(index.x);
            
            Vector2Int startIndex = GetStartIndex(row, index, playerId);
            int cellsAhead = width - 1 - startIndex.y;

            if (cellsAhead < 3)
                return false;

            var result = FourInRaw(row, startIndex, playerId);
            // if (result) 
            //     Debug.Log($"<color=green>{playerId} </color> win!");

            return result;
        }

        private Vector2Int GetStartIndex(Cell[] row, Vector2Int index, PlayerId playerId)
        {
            Vector2Int startIndex = index;
            
            for (int y = index.y; y >= 0; y--)
            {
                if (row[y].CellId != playerId)
                    break;

                startIndex = new Vector2Int(index.x, y);
            }

            return startIndex;
        }

        private bool FourInRaw(Cell[] row, Vector2Int index, PlayerId playerId)
        {
            // Debug.Log("\n");
            for (var y = index.y; y < index.y + 4; y++)
            {
                // Debug.Log($"index: {row[y].Index} id: {row[y].CellId}");
                if (row[y].CellId != playerId)
                    return false;
            }

            return true;
        }
    }
}
