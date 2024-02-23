using System.Collections.Generic;
using System.Linq;
using Connect4.Scripts.Commands;
using Connect4.Scripts.Services.VictoryCheckerService;
using Connect4.Scripts.Strategies.IBlockingStrategy;
using Connect4.Scripts.Turns;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurnCalculationsService : ITurnCalculationsService
{
    private readonly IGridService _gridService;
    private readonly ICommandHistoryService _commandHistoryService;
    private readonly IVictoryCheckerService _victoryCheckerService;

    private readonly VerticalBuildingStrategy _verticalBuildingStrategy;
    private readonly HorizontalBuildingStrategy _horizontalBuildingStrategy;
    private readonly DiagonalBuildingStrategy _diagonalBuildingStrategy;

    public TurnCalculationsService(IGridService gridService, ICommandHistoryService commandHistoryService,
        IVictoryCheckerService victoryCheckerService)
    {
        _victoryCheckerService = victoryCheckerService;
        _commandHistoryService = commandHistoryService;
        _gridService = gridService;

        _verticalBuildingStrategy = new VerticalBuildingStrategy(_gridService);
        _horizontalBuildingStrategy = new HorizontalBuildingStrategy(_gridService);
        _diagonalBuildingStrategy = new DiagonalBuildingStrategy(_gridService);
    }

    public Vector2Int GetBestDecisionFor(Player activePlayer)
    {
        if (_commandHistoryService.HasCommands())
        {
            var commands = _commandHistoryService.GetMovesBy(activePlayer.PlayerId);

            if (commands.Count == 0)
                return TakeColumn(GetRandomColumn());

            var wonTurns = GetWonTurns(activePlayer);
            var blockOpponentTurns = GetBlockOpponentTurns();

            if (wonTurns.Count > 0)
            {
                Debug.Log("do won");
                return TakeColumn(wonTurns[Random.Range(0, wonTurns.Count)].Index.y);
            }
            
            if (blockOpponentTurns.Count > 0)
            {
                Debug.Log("do block");
                return TakeColumn(blockOpponentTurns[Random.Range(0, blockOpponentTurns.Count)].Index.y);
            }

            List<Turn> turns = new List<Turn>();

            foreach (var column in _gridService.Columns)
            {
                var command = new MoveCommand(new Vector2Int(column.LastElementIndex, column.Index), activePlayer);
                turns.AddRange(_verticalBuildingStrategy.GetIndexesToBuild(command));
            }
            
            foreach (var command in commands)
            {
                turns.AddRange(_horizontalBuildingStrategy.GetIndexesToBuild(command));
                turns.AddRange(_diagonalBuildingStrategy.GetIndexesToBuild(command));
            }

            var bannedTurns = GetBannedTurns();
            if (turns.Count == 0)
            {
                Debug.Log($"banned count: {bannedTurns.Count}");
                return TakeColumn(GetRandomColumn());
            }

            turns.RemoveAll(turn => bannedTurns.Any(bannedTurn => bannedTurn.Index == turn.Index));
            
            int maxPriority = turns.Max(turn => turn.Priority);
            
            Turn randomTurn = turns
                .Where(x => x.Priority == maxPriority)
                .OrderBy(x => Random.value > 0.5f)
                .FirstOrDefault();
            
            Debug.Log("do build");

            return TakeColumn(randomTurn.Index.y);
        }

        Debug.Log("do random");
        return TakeColumn(GetRandomColumn());
    }

    private List<Turn> GetBannedTurns()
    {
        List<Turn> commands = new List<Turn>();
        ICommand lastCommand = _commandHistoryService.Peek();
        var activePlayer = lastCommand.ActivePlayer;

        for (var i = 0; i < _gridService.Columns.Count; i++)
        {
            if (_gridService.Columns[i].LastElementIndex <= 1)
                continue;

            var index = TakeColumn(i);
            index = TakeColumn(i);

            SimulateTurn(index, activePlayer.PlayerId);

            if (_victoryCheckerService.TurnIsWin(index, activePlayer.PlayerId, out int priority))
            {
                Debug.Log(new Vector2Int(index.x + 1, index.y));
                var bannedCommand = new Turn(new Vector2Int(index.x + 1, index.y));
                commands.Add(bannedCommand);
            }

            CancelSimulate(index);
            ReleaseColumn(i);
            ReleaseColumn(i);
        }

        return commands;
    }

    private List<Turn> GetBlockOpponentTurns()
    {
        List<Turn> commands = new List<Turn>();
        ICommand lastCommand = _commandHistoryService.Peek();
        var activePlayer = lastCommand.ActivePlayer;

        for (var i = 0; i < _gridService.Columns.Count; i++)
        {
            if (_gridService.Columns[i].LastElementIndex == 0)
                continue;

            // ShowRow(_gridService.Columns[i].LastElementIndex);
            // Debug.Log(new Vector2Int(_gridService.Columns[i].LastElementIndex - 1, _gridService.Columns[i].Index));

            var index = TakeColumn(i);
            SimulateTurn(index, activePlayer.PlayerId);

            // ShowRow(_gridService.Columns[i].LastElementIndex);

            if (_victoryCheckerService.TurnIsWin(index, activePlayer.PlayerId, out int priority))
            {
                // Debug.Log("to block");
                var winedCommand = new Turn(index);
                commands.Add(winedCommand);
            }

            CancelSimulate(index);
            ReleaseColumn(i);

            // Debug.Log("\n");
        }

        return commands;
    }

    private List<Turn> GetWonTurns(Player player)
    {
        List<Turn> commands = new List<Turn>();
        var activePlayer = player;

        for (var i = 0; i < _gridService.Columns.Count; i++)
        {
            if (_gridService.Columns[i].LastElementIndex == 0)
                continue;

            var index = TakeColumn(i);
            SimulateTurn(index, player.PlayerId);

            if (_victoryCheckerService.TurnIsWin(index, activePlayer.PlayerId, out int priority))
            {
                var winedCommand = new Turn(index, priority);
                commands.Add(winedCommand);
            }

            CancelSimulate(index);
            ReleaseColumn(i);
            // Debug.Log(_gridService.GetCell(index).CellId);
        }

        return commands;
    }

    private void ShowRow(int lastElementIndex)
    {
        Cell[] row = _gridService.GetRow(lastElementIndex - 1);
        List<string> stringArray = row.Select(cell => cell.CellId.ToString()).ToList();

        string arrayString = string.Join(" ", stringArray);
        Debug.Log(arrayString);
    }


    private void SimulateTurn(Vector2Int index, PlayerId playerId) =>
        _gridService.TakeCell(index.x, index.y, playerId);

    private void CancelSimulate(Vector2Int index) =>
        _gridService.ReleaseCell(index.x, index.y);

    List<ICommand> RemoveCellsWithout(PlayerId playerId, List<ICommand> commands)
    {
        for (int i = commands.Count - 1; i >= 0; i--)
            if (_gridService.GetCell(commands[i].Index.x, commands[i].Index.y).CellId != playerId)
                commands.RemoveAt(i);

        return commands;
    }

    private int GetRandomColumn()
    {
        var columns = _gridService.Columns.Where(x => x.HasFreeCell).ToList();
        var randomColumn = Random.Range(0, columns.Count - 1);

        return columns[randomColumn].Index;
    }

    private Vector2Int TakeColumn(int index)
    {
        var column = _gridService.Columns[index];
        column.AddElement();

        return new Vector2Int(column.LastElementIndex, column.Index);
    }

    private void ReleaseColumn(int index)
    {
        var column = _gridService.Columns[index];
        column.RemoveElement();
    }
}

public interface ITurnCalculationsService
{
    Vector2Int GetBestDecisionFor(Player activePlayer);
}