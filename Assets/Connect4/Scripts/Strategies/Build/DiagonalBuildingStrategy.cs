using System.Collections.Generic;
using System.Linq;
using Connect4.Scripts.Commands;
using Connect4.Scripts.Field;
using Connect4.Scripts.Services.GridService;
using Connect4.Scripts.Turns;
using UnityEngine;

namespace Connect4.Scripts.Strategies.Build
{
    public class DiagonalBuildingStrategy : BaseStrategy, ILineBuildingStrategy
    {
        public DiagonalBuildingStrategy(IGridService gridService) : base(gridService)
        {
            _gridService = gridService;
        }

        public List<Turn> GetIndexesToBuild(ICommand command)
        {
            List<Turn> indexesToBuild = new List<Turn>();
            
            indexesToBuild.AddRange(GetDiagonalIndexes(command, 2).Select(index => new Turn(index, 2)));

            if (indexesToBuild.Count == 0) 
                indexesToBuild.AddRange(GetDiagonalIndexes(command, 1).Select(index => new Turn(index, 1)));

            return indexesToBuild;
        }

        private List<Vector2Int> GetDiagonalIndexes(ICommand command, int streak)
        {
            List<Vector2Int> indexesToBuild = new List<Vector2Int>();
            indexesToBuild.AddRange(GetDiagonalToBuild(command, streak));

            return indexesToBuild;
        }

        private List<Vector2Int> GetDiagonalToBuild(ICommand command, int streak)
        {
            List<Vector2Int> toBuild = new List<Vector2Int>();
        
            toBuild.AddRange(GetBuildIndexes(GetRightDiagonal(command.Index)));
            toBuild.AddRange(GetBuildIndexes(GetLeftDiagonal(command.Index)));

            // toBuild.ForEach(x => Debug.Log(x));

            return toBuild;
        
            List<Vector2Int> GetBuildIndexes(List<Cell> diagonal)
            {
                return HasFreeCell(diagonal) && GetOpponentStreak(command, diagonal.ToArray(), out List<Vector2Int> indexes, streak)
                    ? indexes
                    : new List<Vector2Int>();
            }
        }

        private bool HasFreeCell(List<Cell> diagonal)
        {
            int count = 0;
            
            foreach (var x in diagonal)
                if (x.CellId == PlayerId.Empty)
                    count++;

            return count > 0;
        }

        private List<Cell> GetRightDiagonal(Vector2Int index)
        {
            var startIndex = GetRightDiagonalStartIndex(index);
            return GetDiagonal(startIndex, 1);
        }

        private List<Cell> GetLeftDiagonal(Vector2Int index)
        {
            var startIndex = GetLeftDiagonalStartIndex(index);
            return GetDiagonal(startIndex, -1);
        }

        private Vector2Int GetRightDiagonalStartIndex(Vector2Int index) => 
            GetStartIndex(index, 1);
        
        private Vector2Int GetLeftDiagonalStartIndex(Vector2Int index) => 
            GetStartIndex(index, 1);
        
        private Vector2Int GetStartIndex(Vector2Int index, int stepJ)
        {
            Vector2Int startIndex = index;

            for (int i = index.x, j = index.y; IndexInOfRange(i, j); i++, j -= stepJ)
                startIndex = new Vector2Int(i, j);
            
            return startIndex;
        }
    }
}