using System.Threading.Tasks;
using UnityEngine;

public class MoveVisualizer : IMoveVisualizer
{
    private const string PiecePath = "Prefabs/Piece";
    
    private readonly IGameCurator _gameCurator;


    public MoveVisualizer(IGameCurator gameCurator)
    {
        _gameCurator = gameCurator;
    }
    
    public async Task ShowTurn(Vector2 position)
    {
        var prefab = Resources.Load<Piece>(PiecePath);
        var piece = Object.Instantiate(prefab);
        piece.transform.position = new Vector2(position.x, 5f);
        piece.GetComponent<SpriteRenderer>().color = _gameCurator.ActivePlayer.Color;

        var task = piece.MoveTo(position, 0.25f);
        await task;
    }
}

public interface IMoveVisualizer
{
    Task ShowTurn(Vector2 position);
}
