using System.Collections.Generic;
using System.Linq;
using Connect4.Scripts.Commands;
using Connect4.Scripts.Extensions;
using Connect4.Scripts.Field;
using Connect4.Scripts.Services.CommandHistoryService;
using Connect4.Scripts.Services.GridService;
using Connect4.Scripts.Services.VictoryCheckerService;
using Connect4.Scripts.Strategies.Build;
using Connect4.Scripts.Turns;
using UnityEngine;

namespace Connect4.Scripts.Services.TurnCalculationsService
{
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

        public Vector2Int GetBestDecisionFor(Player.Player activePlayer)
        {
            if (_commandHistoryService.HasCommands())
            {
                var commands = _commandHistoryService.GetMovesBy(activePlayer.PlayerId);

                if (commands.Count == 0)
                    return GetIndex(GetRandomColumn());

                var wonTurns = GetWonTurns(activePlayer);
                var blockOpponentTurns = GetBlockOpponentTurns();

                if (wonTurns.Count > 0)
                {
                    Debug.Log("do won");
                    return GetIndex(wonTurns[Random.Range(0, wonTurns.Count)].Index.y);
                }
            
                if (blockOpponentTurns.Count > 0)
                {
                    Debug.Log("do block");
                    return GetIndex(blockOpponentTurns[Random.Range(0, blockOpponentTurns.Count)].Index.y);
                }

                var buildTurns = new List<Turn>();

                buildTurns = GetBuildTurns(activePlayer, commands);
                buildTurns = TryToRemoveDuplicates(buildTurns);

                var bannedTurns = TryToBanTurns(activePlayer, buildTurns);

                if (buildTurns.Count == 0)
                    return GetIndex(GetRandomColumn(bannedTurns));

                if (_gridService.Columns.Count(x => x.HasFreeCell) > 1)
                    buildTurns.RemoveAll(turn => bannedTurns.Any(bannedTurn => bannedTurn.Index == turn.Index));


                if (buildTurns.Count == 0)
                    return GetIndex(bannedTurns.Count > 0
                        ? GetRandomColumnExcept(bannedTurns)
                        : GetRandomColumn());


                int maxPriority = buildTurns.Max(turn => turn.Priority);
                Turn randomTurn = buildTurns
                    .Where(x => x.Priority == maxPriority)
                    .OrderBy(x => Random.value > 0.5f)
                    .FirstOrDefault();
                Debug.Log("do build");
                return GetIndex(randomTurn.Index.y);
            }

            Debug.Log("do random");
            return GetIndex(GetRandomColumn());
        }

        private int GetRandomColumn(List<Turn> bannedTurns) =>
            bannedTurns.Count > 0 
                ? GetRandomColumnExcept(bannedTurns) 
                : GetRandomColumn();

        private int GetRandomColumn()
        {
            var columns = _gridService.Columns.Where(x => x.HasFreeCell).ToList();
            return columns.Random().Index;
        }
    
        private int GetRandomColumnExcept(List<Turn> bannedTurns)
        {
            var freeColumns = _gridService.Columns.Where(x => x.HasFreeCell).ToList();
            var bannedColumns = bannedTurns.Select(x => x.Index.y).ToList();

            if (freeColumns.Count - bannedColumns.Count <= 0)
                return freeColumns.Random().Index;

            var availableColumns = freeColumns
                .Where(x => !bannedColumns.Contains(x.Index));
            
            return availableColumns.Random().Index;
        }

        private List<Turn> GetBuildTurns(Player.Player activePlayer, List<ICommand> commands)
        {
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

            return turns;
        }

        private List<Turn> TryToRemoveDuplicates(List<Turn> turns) => turns
            .GroupBy(turn => turn.Index)
            .Select(group => group.OrderByDescending(turn => turn.Priority).First())
            .ToList();

        private List<Turn> TryToBanTurns(Player.Player activePlayer, List<Turn> turns)
        {
            var bannedTurns = GetBannedTurns(activePlayer);
            turns.RemoveAll(x => _gridService.GetCell(x.Index).CellId != PlayerId.Empty); // for what?
        
            return bannedTurns;
        }

        private Vector2Int GetIndex(int index) => 
            new Vector2Int(_gridService.Columns[index].LastElementIndex, index);

        private List<Turn> GetBannedTurns(Player.Player activePlayer)
        {
            List<Turn> commands = new List<Turn>();
            ICommand lastCommand = _commandHistoryService.Peek();
            var opponent = lastCommand.ActivePlayer;

            for (var i = 0; i < _gridService.Columns.Count; i++)
            {
                if (_gridService.Columns[i].LastElementIndex <= 1)
                    continue;
            
            
                var index = new Vector2Int(_gridService.Columns[i].LastElementIndex - 1, i);
                SimulateTurn(index, activePlayer.PlayerId);
                if (_victoryCheckerService.TurnIsWin(index, activePlayer.PlayerId, out int priority))
                {
                    Debug.Log(new Vector2Int(index.x, index.y) + "yes!");
                    Debug.Log(new Vector2Int(index.x + 1, index.y) + "to bun!");
                    var bannedTurn = new Turn(new Vector2Int(index.x + 1, index.y));
                    commands.Add(bannedTurn);
                }
                CancelSimulate(index);
            
            
            
                index = new Vector2Int(_gridService.Columns[i].LastElementIndex - 1, i);
                SimulateTurn(index, opponent.PlayerId);
                if (_victoryCheckerService.TurnIsWin(index, opponent.PlayerId, out priority))
                {
                    Debug.Log(new Vector2Int(index.x, index.y) + "yes!");
                    Debug.Log(new Vector2Int(index.x + 1, index.y) + "to bun!");
                    var bannedTurn = new Turn(new Vector2Int(index.x + 1, index.y));
                    commands.Add(bannedTurn);
                }
                CancelSimulate(index);

                index = new Vector2Int(_gridService.Columns[i].LastElementIndex - 2, i);
                SimulateTurn(index, opponent.PlayerId);
                if (_victoryCheckerService.TurnIsWin(index, opponent.PlayerId, out priority))
                {
                    Debug.Log(new Vector2Int(index.x, index.y) + "yes!");
                    Debug.Log(new Vector2Int(index.x + 1, index.y) + "to bun!");
                    var bannedTurn = new Turn(new Vector2Int(index.x + 1, index.y));
                    commands.Add(bannedTurn);
                }
                CancelSimulate(index);
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

                var index = new Vector2Int(_gridService.Columns[i].LastElementIndex - 1, i);
                SimulateTurn(index, activePlayer.PlayerId);

                // ShowRow(_gridService.Columns[i].LastElementIndex);

                if (_victoryCheckerService.TurnIsWin(index, activePlayer.PlayerId, out int priority))
                {
                    // Debug.Log("to block");
                    var winedCommand = new Turn(index);
                    commands.Add(winedCommand);
                }

                CancelSimulate(index);

                // Debug.Log("\n");
            }

            return commands;
        }

        private List<Turn> GetWonTurns(Player.Player player)
        {
            List<Turn> commands = new List<Turn>();
            var activePlayer = player;

            for (var i = 0; i < _gridService.Columns.Count; i++)
            {
                if (_gridService.Columns[i].LastElementIndex == 0)
                    continue;

                var index = new Vector2Int(_gridService.Columns[i].LastElementIndex - 1, i);
                SimulateTurn(index, player.PlayerId);

                if (_victoryCheckerService.TurnIsWin(index, activePlayer.PlayerId, out int priority))
                {
                    var winedCommand = new Turn(index, priority);
                    commands.Add(winedCommand);
                }

                CancelSimulate(index);
                // Debug.Log(_gridService.GetCell(index).CellId);
            }

            return commands;
        }
    
        private void SimulateTurn(Vector2Int index, PlayerId playerId)
        {
            _gridService.TakeCell(index.x, index.y, playerId);
            _gridService.TakeColumn(index.y);
        }

        private void CancelSimulate(Vector2Int index)
        {
            _gridService.ReleaseCell(index.x, index.y);
            _gridService.ReleaseColumn(index.y);
        }
    }

    public interface ITurnCalculationsService
    {
        Vector2Int GetBestDecisionFor(Player.Player activePlayer);
    }
}