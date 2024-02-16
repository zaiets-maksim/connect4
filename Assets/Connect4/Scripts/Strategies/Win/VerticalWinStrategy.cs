using System.Collections.Generic;
using UnityEngine;

namespace Connect4.Scripts.Strategies.Win
{
    public class VerticalWinStrategy : IWinningStrategy
    {
        public List<Vector2Int> FourInLine { get; } = new List<Vector2Int>();

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
            FourInLine.Clear();
            for (var i = index; i < index + 4; i++)
            {
                // Debug.Log($"index: {column[i].Index} id: {column[i].CellId}");
                if (column[i].CellId != playerId)
                    return false;
                
                FourInLine.Add(column[i].Index);
            }

            return true;
        }
    }
}
