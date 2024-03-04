using System.Collections.Generic;
using System.Linq;
using Connect4.Scripts.Commands;
using Connect4.Scripts.Field;
using Connect4.Scripts.Services.GridService;
using Connect4.Scripts.Turns;
using UnityEngine;

namespace Connect4.Scripts.Strategies.Build
{
    public class VerticalBuildingStrategy : BaseStrategy, ILineBuildingStrategy
    {
        public VerticalBuildingStrategy(IGridService gridService) : base(gridService)
        {
            _gridService = gridService;
        }
        
        public List<Turn> GetIndexesToBuild(ICommand command)
        {
            List<Turn> indexesToBuild = new List<Turn>();
            indexesToBuild.AddRange(GetVerticalToBuild(command, 2).Select(index => new Turn(index, 2)));
            
            if(indexesToBuild.Count == 0)
                indexesToBuild.AddRange(GetVerticalToBuild(command, 1).Select(index => new Turn(index, 1)));

            return indexesToBuild;
        }
        
        private List<Vector2Int> GetVerticalToBuild(ICommand command, int streak)
        {
            List<Vector2Int> indexes = new List<Vector2Int>();
        
            if (_gridService.Columns[command.Index.y].HasFreeCell)
            {
                var verticalStreak = GetOpponentStreak
                    (command.Index,
                    _gridService.GetColumn(command.Index.y),
                    _gridService.Height, command.ActivePlayer.PlayerId);

                if (verticalStreak == streak)
                {
                    indexes.Add(new Vector2Int(command.Index.x - 1, command.Index.y));
                    // Debug.Log($"verticalsStreak is {verticalStreak}");
                }
            }

            return indexes;
        }
        
        private int GetOpponentStreak(Vector2Int index, Cell[] column, int height, PlayerId playerId)
        {
            int counter = 0;
            var startIndex = index;

            // Debug.Log("\n");
            for (int x = index.x; x < height; x++)
            {
                if (column[x].CellId != playerId)
                    break;
                
                // Debug.Log($"index: {column[x].Index} id: {column[x].CellId}");
                startIndex = column[x].Index;
                ++counter;
            }

            return startIndex.x >= 3 ? counter : 0;
        }
    }
}
