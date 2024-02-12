using Connect4.Scripts.Services.VictoryCheckerService;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameCurator : IGameCurator
{
    private Player _player1, _player2;
    private IMoveVisualizer _moveVisualizer;
    private IGridService _gridService;
    private IVictoryCheckerService _victoryCheckerService;
    private ITurnCalculationsService _turnCalculationsService;
    private ICommandHistoryService _commandHistoryService;


    public Player ActivePlayer { get; private set; }

    public void Initialize(IMoveVisualizer moveVisualizer, IGridService gridService, 
        IVictoryCheckerService victoryCheckerService, ITurnCalculationsService turnCalculationsService, 
        ICommandHistoryService commandHistoryService)
    {
        _commandHistoryService = commandHistoryService;
        _turnCalculationsService = turnCalculationsService;
        _victoryCheckerService = victoryCheckerService;
        _gridService = gridService;
        _moveVisualizer = moveVisualizer;
    }

    private void SetActivePlayer(Player player)
    {
        _player1.IsReady = false;
        _player2.IsReady = false;

        ActivePlayer = player;
        
        ActivePlayer.AwaitTurn();
    }

    public void SetPlayers<T>(T player1, T player2) where T : Player
    {
        _player1 = player1;
        _player2 = player2;

        (Color color1, Color color2) = (Color.red, Color.blue);
        
        if (Random.value > 0.5f)
            (color1, color2) = (color2, color1);

        _player1.Initialize(
            this, _moveVisualizer, _gridService, _victoryCheckerService, _turnCalculationsService,
            _commandHistoryService, PlayerId.Player1, color1);
        _player2.Initialize(this, _moveVisualizer, _gridService, _victoryCheckerService, _turnCalculationsService,
            _commandHistoryService, PlayerId.Player2, color2);
        
        SetActivePlayer(Random.value > 0.5f ? _player1 : _player2);
    }

    public void SwitchPlayers() => 
        SetActivePlayer(ActivePlayer == _player1 ? _player2 : _player1);
    
    public void EndTurn()
    {
        SwitchPlayers();        
    }
}

public interface IGameCurator
{
    Player ActivePlayer { get; }
    void SetPlayers<T>(T player1, T player2) where T : Player;
    void SwitchPlayers();
    void EndTurn();
    void Initialize(IMoveVisualizer moveVisualizer, IGridService gridService, IVictoryCheckerService victoryCheckerService, ITurnCalculationsService turnCalculationsService, 
        ICommandHistoryService commandHistoryService);

}
