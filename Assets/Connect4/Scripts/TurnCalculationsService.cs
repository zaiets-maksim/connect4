using System.Collections.Generic;
using System.Linq;
using Connect4.Scripts.Strategies.IBlockingStrategy;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurnCalculationsService : ITurnCalculationsService
{
    private readonly IGridService _gridService;
    private readonly ICommandHistoryService _commandHistoryService;
    private readonly IGameCurator _gameCurator;

    private readonly VerticalBlockStrategy _verticalBlockStrategy;
    private readonly HorizontalBlockStrategy _horizontalBlockStrategy;
    private readonly DiagonalBlockStrategy _diagonalBlockStrategy;

    public TurnCalculationsService(IGridService gridService, ICommandHistoryService commandHistoryService, 
        IGameCurator gameCurator)
    {
        _gameCurator = gameCurator;
        _commandHistoryService = commandHistoryService;
        _gridService = gridService;
        
        _verticalBlockStrategy = new VerticalBlockStrategy(_gridService);
        _horizontalBlockStrategy = new HorizontalBlockStrategy(_gridService);
        _diagonalBlockStrategy = new DiagonalBlockStrategy(_gridService);
    }

    public Vector2Int GetBestDecision()
    {
        if (_commandHistoryService.HasCommands())
        {
            // var commands = _commandHistoryService.GetMovesBy(_gameCurator.ActivePlayer.PlayerId);

            ICommand lastCommand = _commandHistoryService.Peek();

            List<Vector2Int> verticalIndexes = _verticalBlockStrategy.GetIndexesToBlock(lastCommand);
            List<Vector2Int> horizontalIndexes = _horizontalBlockStrategy.GetIndexesToBlock(lastCommand);
            List<Vector2Int> diagonalIndexes = _diagonalBlockStrategy.GetIndexesToBlock(lastCommand);

            var cells = new List<Vector2Int>();
            cells.AddRange(verticalIndexes);
            cells.AddRange(horizontalIndexes);
            cells.AddRange(diagonalIndexes);

            // Debug.Log("\n");
            // return TakeColumn(6);

            var bannedCommands = new List<ICommand>();// skip, if has any other not full column
            bannedCommands.AddRange(_verticalBlockStrategy.BannedCommands);
            bannedCommands.AddRange(_horizontalBlockStrategy.BannedCommands);
            bannedCommands.AddRange(_diagonalBlockStrategy.BannedCommands);
            bannedCommands = RemoveCellsWithout(_gameCurator.ActivePlayer.PlayerId, bannedCommands);

            foreach (var command in bannedCommands)
            {
                if (command.Index == lastCommand.Index)
                {
                    Debug.Log($"obviously! it`s {command.Index} turn!");
                    return TakeColumn(command.Index.y);
                }
            }

            if(_gridService.Columns.Count(x => x.HasFreeCell) > 1)
                cells.RemoveAll(index => bannedCommands.Any(bannedCell => bannedCell.Index == index));

            foreach (var command in bannedCommands) 
                Debug.Log($"BANNED: {command.Index} {command.ActivePlayer.PlayerId}");

            if(cells.Count > 0)
                return TakeColumn(cells[Random.Range(0, cells.Count)].y);
        }

        return TakeColumn(GetRandomColumn());
        return TakeColumn(6);
    }
    
    List<ICommand> RemoveCellsWithout(PlayerId playerId, List<ICommand> commands)
    {
        for (int i = commands.Count - 1; i >= 0; i--)
            if (_gridService.GetCell(commands[i].Index.x, commands[i].Index.y).CellId != playerId)
                commands.RemoveAt(i);
            
        return commands;
    }

    // bannedIndexes = bannedIndexes.Except(RemoveNonEmptyCells(bannedIndexes)).ToList();
    // List<Cell> RemoveNonEmptyCells(List<Cell> bannedIndexes)
    // {
    //     return bannedIndexes
    //         .Where(cell => _gridService.GetCell(cell.Index.x, cell.Index.y).CellId == PlayerId.Empty)
    //         .ToList();
    // }

    private int GetRandomColumn()
    {
        var columns = _gridService.Columns.Where(x => x.HasFreeCell).ToList();
        var randomColumn = Random.Range(0, columns.Count - 1);

        return columns[randomColumn].Index;
    }

    private Vector2Int TakeColumn(int index)
    {
        var column = _gridService.Columns[index];
        column.UpdateLastElementIndex();

        return new Vector2Int(column.LastElementIndex, column.Index);
    }
}

public interface ITurnCalculationsService
{
    Vector2Int GetBestDecision();
}