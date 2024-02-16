using System.Collections.Generic;
using UnityEngine;

namespace Connect4.Scripts.Strategies.Win
{
    public class DiagonalWinStrategy : IWinningStrategy
    {
        public List<Vector2Int> FourInLine { get; } = new List<Vector2Int>();

        public bool IsWinningMove(IGridService gridService, Vector2Int index, PlayerId playerId)
        {
            int height = gridService.Height;
            int width = gridService.Width;

            Diagonal rightDiagonal = new Diagonal
            {
                CellsAhead = Mathf.Min(index.x, width - 1 - index.y),
                CellsBehind = Mathf.Min(index.y, height - 1 - index.x)
            };
            
            Diagonal leftDiagonal = new Diagonal
            {
                CellsAhead = Mathf.Min(index.x, index.y),
                CellsBehind = Mathf.Min(width - 1 - index.y, height - 1 - index.x)
            };

            bool fourInRightDiagonal = false;
            bool fourInLeftDiagonal = false;

            if (rightDiagonal.CellsAhead + rightDiagonal.CellsBehind >= 3)
            {
                rightDiagonal.StartIndex = GetRightDiagonalStartIndex(gridService, playerId, index.x, index.y);
                rightDiagonal.CellsAhead = Mathf.Min(rightDiagonal.StartIndex.x, width - 1 - rightDiagonal.StartIndex.y);

                if (rightDiagonal.CellsAhead >= 3)
                {
                    rightDiagonal.Elements = GetRightDiagonal(rightDiagonal.StartIndex, gridService);
                    fourInRightDiagonal = FourInDiagonal(rightDiagonal.Elements, playerId);
                }
            }
            
            if (leftDiagonal.CellsAhead + leftDiagonal.CellsBehind >= 3)
            {
                leftDiagonal.StartIndex = GetLeftDiagonalStartIndex(gridService, playerId, index.x, index.y);
                leftDiagonal.CellsAhead = Mathf.Min(leftDiagonal.StartIndex.x, leftDiagonal.StartIndex.y);

                if (leftDiagonal.CellsAhead >= 3)
                {
                    leftDiagonal.Elements = GetLeftDiagonal(leftDiagonal.StartIndex, gridService);
                    fourInLeftDiagonal = FourInDiagonal(leftDiagonal.Elements, playerId);
                }
            }

            var result = fourInRightDiagonal || fourInLeftDiagonal;

            // if (result)
            //     Debug.Log($"<color=green>{playerId} </color> win!");

            return result;
        }

        private Vector2Int GetRightDiagonalStartIndex(IGridService gridService, PlayerId playerId, int i, int j) => 
            GetDiagonalStartIndex(gridService, playerId, i, j, -1);
        
        private Vector2Int GetLeftDiagonalStartIndex(IGridService gridService, PlayerId playerId, int i, int j) => 
            GetDiagonalStartIndex(gridService, playerId, i, j, 1);

        private Vector2Int GetDiagonalStartIndex(IGridService gridService, PlayerId playerId, int i, int j, int stepJ)
        {
            Vector2Int startIndex = new Vector2Int(i, j);

            for (; IndexInOfRange(gridService, i, j); i++, j += stepJ)
            {
                if (gridService.Grid[i][j].CellId != playerId)
                    break;

                startIndex = new Vector2Int(i, j);
            }
            
            return startIndex;
        }

        private List<Cell> GetRightDiagonal(Vector2Int index, IGridService gridService) => 
            GetDiagonal(index, gridService, 1);

        private List<Cell> GetLeftDiagonal(Vector2Int index, IGridService gridService) => 
            GetDiagonal(index, gridService, -1);
        
        private List<Cell> GetDiagonal(Vector2Int index, IGridService gridService, int stepJ)
        {
            var diagonal = new List<Cell>();

            for (int i = index.x, j = index.y; IndexInOfRange(gridService, i, j); i--, j += stepJ)
                diagonal.Add(gridService.Grid[i][j]);

            return diagonal;
        }

        private bool FourInDiagonal(List<Cell> diagonal, PlayerId playerId)
        {
            // Debug.Log("\n");
            FourInLine.Clear();
            for (var i = 0; i < 4; i++)
            {
                // Debug.Log($"index: {diagonal[i].Index} id: {diagonal[i].CellId}");
                if (diagonal[i].CellId != playerId)
                    return false;
                
                FourInLine.Add(diagonal[i].Index);
            }
            
            return true;
        }
        
        private bool IndexInOfRange(IGridService gridService, int i, int j) => 
            i >= 0 && j >= 0 && i < gridService.Height && j < gridService.Width;
        
        class Diagonal
        {
            public int CellsAhead;
            public  int CellsBehind;
            public Vector2Int StartIndex;
            public  List<Cell> Elements = new List<Cell>();
        }
    }
}
