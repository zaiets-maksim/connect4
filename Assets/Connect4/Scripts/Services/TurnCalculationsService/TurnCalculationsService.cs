using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public int Depth { get; set; } = 3;
        public bool UseStandardMiniMax { get; set; }

        private PlayerId _aiPlayer;
        private PlayerId _opponent;

        public Vector2Int GetBestDecisionFor(Player.Player activePlayer)
        {
            _aiPlayer = activePlayer.PlayerId;
            _opponent = _aiPlayer == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;

            var board = _gridService.CloneGrid();

            var x = Task.Run(() => GetNextMove(board)).Result;

            return GetIndex(x);
        }
        
        public int GetNextMove(Cell[][] board)
        {
            if (UseStandardMiniMax)
            {
                var (column, score) = MinimaxStandard(board, Depth, true);
                return column;
            }
            else
            {
                var (column, score) = Minimax(board, Depth, int.MinValue, int.MaxValue, true);
                return column;
            }
        }

        private bool PlayerWon(Cell[][] board, PlayerId player)
        {
            for (var i = 0; i < board.Length; i++)
            {
                for (var j = 0; j < board[i].Length; j++)
                {
                    if (_victoryCheckerService.TurnIsWin(new Vector2Int(i, j), player))
                        return true;
                }
            }

            return false;
        }

        private int MakeMove(Cell[][] board, int column, PlayerId currentPlayer)
        {
            if (board[0][column].CellId != PlayerId.Empty)
            {
                return -1;
            }

            var row = board.GetLength(0) - 1;
            while (board[row][column].CellId != PlayerId.Empty)
            {
                row--;
            }

            board[row][column].CellId = currentPlayer;

            return row;
        }
        
        private void UndoMove(Cell[][] board, int column)
        {
            //empty column
            if (board[board.GetLength(0) - 1][column].CellId == PlayerId.Empty)
                return;

            var row = 0;
            while (board[row][column].CellId == PlayerId.Empty)
            {
                row++;
            }

            board[row][column].CellId = PlayerId.Empty;
        }

        private (int col, int score) Minimax(Cell[][] board, int depth, int alpha, int beta, bool isMaximizingPlayer)
        {
            var terminalNode = TerminalNode(board);
            if (depth == 0 || terminalNode)
            {
                if (!terminalNode)
                    return (-1, EvaluateBoard(board));
                if (PlayerWon(board, _aiPlayer))
                    return (-1, 4000000);
                if (PlayerWon(board, _opponent))
                    return (-1, -4000000);

                return (-1, 0);
            }

            var height = _gridService.Height;
            var length = _gridService.Width;

            if (isMaximizingPlayer)
            {
                var bestScore = int.MinValue;
                var bestColumn = 0;
                for (var column = 0; column < length; column++)
                {
                    var result = MakeMove(board, column, _aiPlayer);
                    if (result == -1)
                        continue;

                    var newScore = Minimax(board, depth - 1, alpha, beta, false).score;
                    if (newScore > bestScore)
                    {
                        bestColumn = column;
                        bestScore = newScore;
                    }

                    UndoMove(board, column);
                    alpha = Mathf.Max(alpha, bestScore);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return (bestColumn, bestScore);
            }
            else
            {
                var bestScore = int.MaxValue;
                var bestColumn = 0;
                for (var column = 0; column < length; column++)
                {
                    var result = MakeMove(board, column, _opponent);
                    if (result == -1)
                        continue;

                    var newScore = Minimax(board, depth - 1, alpha, beta, true).score;
                    if (newScore < bestScore)
                    {
                        bestColumn = column;
                        bestScore = newScore;
                    }

                    UndoMove(board, column);
                    beta = Mathf.Min(beta, bestScore);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return (bestColumn, bestScore);
            }
        }

        private (int col, int score) MinimaxStandard(Cell[][] board, int depth, bool isMaximizingPlayer)
        {
            var terminalNode = TerminalNode(board);
            if (depth == 0 || terminalNode)
            {
                if (!terminalNode)
                    return (-1, EvaluateBoard(board));
                if (PlayerWon(board, _aiPlayer))
                    return (-1, 4000000);
                if (PlayerWon(board, _opponent))
                    return (-1, -4000000);

                return (-1, 0);
            }

            var height = _gridService.Height;
            var length = _gridService.Width;

            if (isMaximizingPlayer)
            {
                var bestScore = int.MinValue;
                var bestColumn = 0;
                for (var column = 0; column < length; column++)
                {
                    var result = MakeMove(board, column, _aiPlayer);
                    if (result == -1)
                        continue;

                    var newScore = MinimaxStandard(board, depth - 1, false).score;
                    if (newScore > bestScore)
                    {
                        bestColumn = column;
                        bestScore = newScore;
                    }

                    UndoMove(board, column);
                }

                return (bestColumn, bestScore);
            }
            else
            {
                var bestScore = int.MaxValue;
                var bestColumn = 0;
                for (var column = 0; column < length; column++)
                {
                    var result = MakeMove(board, column, _opponent);
                    if (result == -1)
                        continue;

                    var newScore = MinimaxStandard(board, depth - 1, true).score;
                    if (newScore < bestScore)
                    {
                        bestColumn = column;
                        bestScore = newScore;
                    }

                    UndoMove(board, column);
                }

                return (bestColumn, bestScore);
            }
        }

        private int GetEmptyRowNumber(Cell[][] board, int column)
        {
            var row = board.GetLength(0) - 1;
            while (board[row][column].CellId != PlayerId.Empty)
            {
                row--;

                if (row == -1)
                {
                    return -1;
                }
            }

            return row;
        }
        
        private bool TerminalNode(Cell[][] board)
        {
            if (PlayerWon(board, _aiPlayer) || PlayerWon(board, _opponent))
                return true;

            var list = new List<int>();
            for (var i = 0; i < board.Length; i++)
            {
                list.Add(GetEmptyRowNumber(board, i));
            }

            return list.All(x => x == -1);
        }

        private int EvaluateBoard(Cell[][] board)
        {
            var score = 0;
            var rows = board.Length;
            var cols = board[0].Length;

            // Prefer center column
            for (var i = 0; i < rows; i++)
            {
                if (board[i][(int) Mathf.Floor(cols / 2.0f)].CellId == _aiPlayer)
                    score += 4;
            }

            var listSize = 4;
            //Score horizontals
            for (var i = 0; i < rows; i++)
            {
                var row = board[i].ToList();

                for (int c = 0; c < row.Count - listSize + 1; c++)
                {
                    score += Evaluate4Elements(row.Skip(c).Take(listSize).Select(x => x.CellId).ToList());
                }
            }

            //Score vertical
            for (var i = 0; i < cols; i++)
            {
                var column = board.Select(row => row[i].CellId).ToList();

                for (int r = 0; r < column.Count - listSize + 1; r++)
                {
                    score += Evaluate4Elements(column.Skip(r).Take(listSize).ToList());
                }
            }

            // diagonals
            for (var i = -cols + 3; i < rows - 2; i++)
            {
                var diagonal = new List<PlayerId>();

                for (var row = 0; row < rows; row++)
                {
                    var col = row - i;

                    if (col >= 0 && col < cols) 
                        diagonal.Add(board[row][col].CellId);
                }

                for (var j = 0; j < diagonal.Count - listSize + 1; j++)
                {
                    score += Evaluate4Elements(diagonal.Skip(j).Take(listSize).ToList());
                }
            }

            // anti diagonals
            for (var i = 3; i < rows + cols - 4; i++)
            {
                var diagonal = new List<PlayerId>();

                for (var row = 0; row < rows; row++)
                {
                    var col = i - row;

                    if (col >= 0 && col < cols) 
                        diagonal.Add(board[row][col].CellId);
                }

                for (var j = 0; j < diagonal.Count - listSize + 1; j++)
                {
                    score += Evaluate4Elements(diagonal.Skip(j).Take(listSize).ToList());
                }
            }

            return score;
        }

        private int Evaluate4Elements(List<PlayerId> list)
        {
            var score = 0;

            var aiCount = list.Count(x => x == _aiPlayer);
            var emptyCount = list.Count(x => x == PlayerId.Empty);

            switch (aiCount)
            {
                case 4:
                    score += 1000;
                    break;
                case 3 when emptyCount == 1:
                    score += 10;
                    break;
                case 2 when emptyCount == 2:
                    score += 5;
                    break;
            }

            if (list.Count(x => x == _opponent) == 2 && emptyCount == 2)
                score -= 9;

            return score;
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

                if (TurnNeedToBun(activePlayer, new Vector2Int(_gridService.Columns[i].LastElementIndex - 1, i),
                        out Turn bannedTurn))
                    commands.Add(bannedTurn);

                if (TurnNeedToBun(opponent, new Vector2Int(_gridService.Columns[i].LastElementIndex - 1, i),
                        out bannedTurn))
                    commands.Add(bannedTurn);

                if (TurnNeedToBun(opponent, new Vector2Int(_gridService.Columns[i].LastElementIndex - 2, i),
                        out bannedTurn))
                    commands.Add(bannedTurn);
            }

            return commands;
        }

        private bool TurnNeedToBun(Player.Player player, Vector2Int index, out Turn bannedTurn)
        {
            bannedTurn = new Turn(new Vector2Int());

            SimulateTurn(index, player.PlayerId);
            if (_victoryCheckerService.TurnIsWin(index, player.PlayerId, out int priority))
            {
                Debug.Log(new Vector2Int(index.x, index.y) + "yes!");
                Debug.Log(new Vector2Int(index.x + 1, index.y) + "to bun!");
                bannedTurn = new Turn(new Vector2Int(index.x + 1, index.y));
                CancelSimulate(index);

                return true;
            }

            CancelSimulate(index);

            return false;
        }

        private List<Turn> GetBlockOpponentTurns()
        {
            ICommand lastCommand = _commandHistoryService.Peek();
            PlayerId playerId = lastCommand.ActivePlayer.PlayerId;

            return GetTurns(playerId);
        }

        private List<Turn> GetWonTurns(PlayerId playerId)
        {
            return GetTurns(playerId);
        }

        private List<Turn> GetTurns(PlayerId playerId)
        {
            List<Turn> turns = new List<Turn>();

            for (var i = 0; i < _gridService.Columns.Count; i++)
            {
                if (_gridService.Columns[i].LastElementIndex == 0)
                    continue;

                var index = new Vector2Int(_gridService.Columns[i].LastElementIndex - 1, i);
                SimulateTurn(index, playerId);

                if (_victoryCheckerService.TurnIsWin(index, playerId, out int priority))
                    turns.Add(new Turn(index));

                CancelSimulate(index);
            }

            return turns;
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