using System.Collections.Generic;
using System.Linq;
using Connect4.Scripts.Turns;
using UnityEngine;

namespace Connect4.Scripts.Strategies.IBlockingStrategy
{
    public class HorizontalBuildingStrategy : BaseStrategy, ILineBuildingStrategy
    {
        public HorizontalBuildingStrategy(IGridService gridService, IGameCurator gameCurator) : base(gridService, gameCurator)
        {
            _gameCurator = gameCurator;
            _gridService = gridService;
        }
    
        public List<Turn> GetIndexesToBuild(ICommand command)
        {
            List<Turn> indexesToBuild = new List<Turn>();
            
            indexesToBuild.AddRange(GetHorizontalToBuild(command, 2).Select(index => new Turn(index, 2)));
            
            if(indexesToBuild.Count == 0)
                indexesToBuild.AddRange(GetHorizontalToBuild(command, 1).Select(index => new Turn(index, 1)));


            return indexesToBuild;
        }
    
        private List<Vector2Int> GetHorizontalToBuild(ICommand command, int streak)
        {
            List<Vector2Int> toBuild = new List<Vector2Int>();
            int rowIndex = command.Index.x;

            if (HasStreak(command, rowIndex, out List<Vector2Int> indexes, streak))
                toBuild.AddRange(indexes);

            // if (rowIndex > 0 && HasStreak(command, rowIndex - 1, out indexes, streak))
            //     toBuild.AddRange(indexes);

            return toBuild;
        }
        
        private bool HasStreak(ICommand command, int row, out List<Vector2Int> indexes, int streak)
        {
            indexes = new List<Vector2Int>();
            
            if(!_gridService.RawHasFreeCell(_gridService.GetRow(row))) 
                return false;

            if (GetOpponentStreak(command, _gridService.GetRow(row), out var list2, streak))
            {
                indexes = list2;
                return true;   
            }

            return false;
        }
    }
}
