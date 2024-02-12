using System.Threading.Tasks;
using Infrastructure.Services.Factories.Game;
using UnityEngine;

public class MoveVisualizer : IMoveVisualizer
{
    private readonly IGameFactory _gameFactory;

    public MoveVisualizer(IGameFactory gameFactory)
    {
        _gameFactory = gameFactory;
    }

    public async Task ShowTurn(Vector2 position, Color color)
    {
        var piece = _gameFactory.CreatePiece(new Vector2(position.x, 5f), color);
        var task = piece.MoveTo(position, 0.25f);
        await task;
    }

    // public async Task CancelTurn(MoveCommand command)
    // {
    //     var piece = command.Piece;
    //     await piece.MoveTo(new Vector2(piece.transform.position.x, 5f), 0.25f);
    //     piece.gameObject.SetActive(false);
    // }
}

public interface IMoveVisualizer
{
    Task ShowTurn(Vector2 position, Color color);
    
    // Task CancelTurn(MoveCommand command);
}
