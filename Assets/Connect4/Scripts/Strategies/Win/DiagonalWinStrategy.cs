using System.Collections.Generic;
using Connect4.Scripts.Field;
using Connect4.Scripts.Services.GridService;
using UnityEngine;

namespace Connect4.Scripts.Strategies.Win
{
    public class DiagonalWinStrategy : BaseStrategy, IWinningStrategy
    {
        public DiagonalWinStrategy(IGridService gridService) : base(gridService) => 
            _gridService = gridService;

        public List<Vector2Int> FourInLine { get; } = new List<Vector2Int>();

        public bool IsWinningMove(Vector2Int index, PlayerId playerId)
        {
            int height = _gridService.Height;
            int width = _gridService.Width;

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

            if (rightDiagonal.CellsAhead + rightDiagonal.CellsBehind >= 3)
            {
                rightDiagonal.StartIndex = GetRightDiagonalStartIndex(playerId, index);
                rightDiagonal.CellsAhead = Mathf.Min(rightDiagonal.StartIndex.x, width - 1 - rightDiagonal.StartIndex.y);

                if (rightDiagonal.CellsAhead >= 3)
                {
                    rightDiagonal.Elements = GetRightDiagonal(rightDiagonal.StartIndex);
                    
                    if (FourInDiagonal(rightDiagonal.Elements, playerId))
                        return true;
                }
            }
            
            if (leftDiagonal.CellsAhead + leftDiagonal.CellsBehind >= 3)
            {
                leftDiagonal.StartIndex = GetLeftDiagonalStartIndex(playerId, index);
                leftDiagonal.CellsAhead = Mathf.Min(leftDiagonal.StartIndex.x, leftDiagonal.StartIndex.y);

                if (leftDiagonal.CellsAhead >= 3)
                {
                    leftDiagonal.Elements = GetLeftDiagonal(leftDiagonal.StartIndex);
                    
                    if (FourInDiagonal(leftDiagonal.Elements, playerId))
                        return true;
                }
            }

            return false;
        }

        private Vector2Int GetRightDiagonalStartIndex(PlayerId playerId, Vector2Int index) => 
            GetDiagonalStartIndex(playerId, index, 1);
        
        private Vector2Int GetLeftDiagonalStartIndex( PlayerId playerId, Vector2Int index) => 
            GetDiagonalStartIndex(playerId, index, -1);

        private Vector2Int GetDiagonalStartIndex(PlayerId playerId, Vector2Int index, int stepJ)
        {
            Vector2Int startIndex = index;

            for (int i = index.x, j = index.y; IndexInOfRange(i, j); i++, j -= stepJ)
            {
                if (_gridService.Grid[i][j].CellId != playerId)
                    break;

                startIndex = new Vector2Int(i, j);
            }
            
            return startIndex;
        }

        private List<Cell> GetRightDiagonal(Vector2Int index) => 
            GetDiagonal(index, 1);

        private List<Cell> GetLeftDiagonal(Vector2Int index) => 
            GetDiagonal(index, -1);

        private bool FourInDiagonal(List<Cell> diagonal, PlayerId playerId)
        {
            // Debug.Log("\n");
            FourInLine.Clear();
            for (var i = 0; i < WinStreak; i++)
            {
                // Debug.Log($"index: {diagonal[i].Index} id: {diagonal[i].CellId}");
                if (diagonal[i].CellId != playerId)
                    return false;
                
                FourInLine.Add(diagonal[i].Index);
            }
            
            return true;
        }

        class Diagonal
        {
            public int CellsAhead;
            public  int CellsBehind;
            public Vector2Int StartIndex;
            public  List<Cell> Elements = new List<Cell>();
        }
    }
}
