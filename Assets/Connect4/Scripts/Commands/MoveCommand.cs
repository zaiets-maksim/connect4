using System.Threading.Tasks;
using UnityEngine;

namespace Connect4.Scripts.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly ICommandHistoryService _commandHistoryService;
        private readonly IGridService _gridService;
        private readonly IMoveVisualizer _moveVisualizer;

        public Vector2Int Index { get; }
        public Player ActivePlayer { get; }
        
        public MoveCommand(Vector2Int index, Player activePlayer, ICommandHistoryService commandHistoryService, IGridService gridService,
            IMoveVisualizer moveVisualizer)
        {
            _moveVisualizer = moveVisualizer;
            _gridService = gridService;
            _commandHistoryService = commandHistoryService;
            ActivePlayer = activePlayer;
            Index = index;
        }

        public MoveCommand(Vector2Int index, Player activePlayer)
        {
            ActivePlayer = activePlayer;
            Index = index;
        }

        public async Task Execute()
        {
            Debug.Log($"{ActivePlayer.PlayerId} is making a move");
        
            await _moveVisualizer.ShowTurn(_gridService.GetCell(Index.x, Index.y).Position, ActivePlayer.Color);
            _gridService.TakeCell(Index.x, Index.y, ActivePlayer.PlayerId);
        
            Debug.Log($"{ActivePlayer.PlayerId} made a move");
        }

        public async void Undo()
        {
            ICommand lastCommand = _commandHistoryService.Pop();

            var index = lastCommand.Index;
            await _moveVisualizer.ShowTurn(_gridService.GetCell(index.x, index.y).Position, ActivePlayer.Color); // wrong!!!
            _gridService.ReleaseCell(Index.x, Index.y);
        }

        public void ToHistory() => 
            _commandHistoryService.Push(this);
    }
}
