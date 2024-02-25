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
        public Piece Piece { get; set; }

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

            var position = _gridService.GetCell(Index.x, Index.y).Position;
            await _moveVisualizer.ShowTurn(position, this);
            _gridService.TakeCell(Index.x, Index.y, ActivePlayer.PlayerId);
        
            Debug.Log($"{ActivePlayer.PlayerId} made a move");
        }

        public async void Undo()
        {
            _gridService.ReleaseCell(Index.x, Index.y);
            _gridService.ReleaseColumn(Index.y);
            await _moveVisualizer.CancelTurn(Piece);
        }

        public void ToHistory() => 
            _commandHistoryService.Push(this);
    }
}
