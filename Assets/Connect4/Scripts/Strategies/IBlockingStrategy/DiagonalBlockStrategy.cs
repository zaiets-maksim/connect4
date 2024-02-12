using System.Collections.Generic;
using Connect4.Scripts.Commands;
using Connect4.Scripts.Strategies;
using Connect4.Scripts.Strategies.IBlockingStrategy;
using UnityEngine;

public class DiagonalBlockStrategy : BaseStrategy, IBlockingStrategy
{
    private readonly IGridService _gridService;

    public DiagonalBlockStrategy(IGridService gridService) : base(gridService)
    {
        _gridService = gridService;
    }

    public List<Vector2Int> GetIndexesToBlock(ICommand command)
    {
        Debug.Log($"MAIN INDEX: {command.Index}");
        List<Vector2Int> indexesToBlock = GetDiagonalToBlock(command);
        
        if (command.Index.x != 0)
        {
            var index = new Vector2Int(command.Index.x - 1, command.Index.y);
            
            Debug.Log($"MAIN INDEX: {index}");

            ICommand newCommand = new MoveCommand(index, command.ActivePlayer);
            indexesToBlock.AddRange(GetDiagonalToBlock(newCommand));
        }

        return indexesToBlock;
    }

    private List<Vector2Int> GetDiagonalToBlock(ICommand command)
    {
        List<Vector2Int> toBlock = new List<Vector2Int>();
        
        ProcessDiagonal(GetRightDiagonal(command.Index));
        ProcessDiagonal(GetLeftDiagonal(command.Index));

        return toBlock;
        
        void ProcessDiagonal(List<Cell> diagonal)
        {
            if (HasFreeCell(diagonal) && GetOpponentStreak(command, diagonal.ToArray(), out var streak))
                toBlock.AddRange(streak);
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