using System.Threading.Tasks;
using Connect4.Scripts.Commands;
using Connect4.Scripts.Field;
using Connect4.Scripts.Services.Factories.Game;
using UnityEngine;

namespace Connect4.Scripts.Services.MoveVisualizer
{
    public class MoveVisualizer : IMoveVisualizer
    {
        private readonly IGameFactory _gameFactory;

        public MoveVisualizer(IGameFactory gameFactory)
        {
            _gameFactory = gameFactory;
        }

        public async Task ShowTurn(Vector2 position, MoveCommand command)
        {
            var piece = _gameFactory.CreatePiece(new Vector2(position.x, 5f), command.ActivePlayer.Color);
            var task = piece.MoveTo(position, 0.25f);
            command.Piece = piece;
            
            await task;
        }

        public async Task CancelTurn(Piece piece)
        {
            await piece.Hide(0.5f);
        }
    }

    public interface IMoveVisualizer
    {
        Task ShowTurn(Vector2 position, MoveCommand command);
    
        Task CancelTurn(Piece piece);
    }
}