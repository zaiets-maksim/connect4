using System.Collections.Generic;
using System.Linq;
using Connect4.Scripts.Commands;
using Connect4.Scripts.Turns;
using UnityEngine;

namespace Connect4.Scripts.Strategies.IBlockingStrategy
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
            // Debug.Log($"MAIN INDEX: {command.Index}");
            
            List<Vector2Int> indexesToBuild = new List<Vector2Int>();
            indexesToBuild.AddRange(GetDiagonalToBuild(command, streak));


            // if (command.Index.x > 0)
            // {
            //     var index = new Vector2Int(command.Index.x - 1, command.Index.y);
            //     ICommand newCommand = new MoveCommand(index, command.ActivePlayer);
            //     indexesToBuild.AddRange(GetDiagonalToBuild(newCommand, streak));
            //
            //     Debug.Log($"MAIN INDEX: {index}");
            // }

            return indexesToBuild;
        }


        private List<Vector2Int> GetDiagonalToBuild(ICommand command, int streak)
        {
            List<Vector2Int> toBuild = new List<Vector2Int>();
        
            ProcessDiagonal(GetRightDiagonal(command.Index));
            ProcessDiagonal(GetLeftDiagonal(command.Index));

            return toBuild;
        
            void ProcessDiagonal(List<Cell> diagonal)
            {
                if (HasFreeCell(diagonal) && GetOpponentStreak(command, diagonal.ToArray(), out List<Vector2Int> indexes, streak))
                    toBuild.AddRange(indexes);
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

        private List<Cell> GetRightDiagonal(Vector2Int index) =>
            GetDiagonal(index, 1);

        private List<Cell> GetLeftDiagonal(Vector2Int index) =>
            GetDiagonal(index, -1);

        private List<Cell> GetDiagonal(Vector2Int index, int stepJ)
        {
            var diagonal = new List<Cell>();
            Vector2Int startIndex = index;

            for (int i = index.x, j = index.y; IndexInOfRange(i, j); i++, j -= stepJ)
                startIndex = new Vector2Int(i, j);

            for (int i = startIndex.x, j = startIndex.y; IndexInOfRange(i, j); i--, j += stepJ)
                diagonal.Add(_gridService.Grid[i][j]);

            return diagonal;
        }
    
        private bool IndexInOfRange(int i, int j) => 
            i >= 0 && j >= 0 && i < _gridService.Height && j < _gridService.Width;
    }
}