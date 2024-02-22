using Infrastructure.Services.Factories.Game;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameCurator : IGameCurator
{
    private Player _player1, _player2;
    private readonly IMoveVisualizer _moveVisualizer;
    private readonly IGridService _gridService;
    private readonly ITurnCalculationsService _turnCalculationsService;
    private readonly ICommandHistoryService _commandHistoryService;
    private readonly IGameFactory _gameFactory;
    private readonly IFinishService _finishService;

    private CurrentTurnViewer _currentTurnViewer => _gameFactory.Hud.CurrentTurnViewer;

    public Player ActivePlayer { get; private set; }

    public GameCurator(IMoveVisualizer moveVisualizer, IGridService gridService, ITurnCalculationsService turnCalculationsService, 
        ICommandHistoryService commandHistoryService, IGameFactory gameFactory, IFinishService finishService)
    {
        _finishService = finishService;
        _gameFactory = gameFactory;
        _commandHistoryService = commandHistoryService;
        _turnCalculationsService = turnCalculationsService;
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
            this, _moveVisualizer, _gridService, _turnCalculationsService,
            _commandHistoryService, _currentTurnViewer, _finishService, PlayerId.Player1, color1);
        
        _player2.Initialize(
            this, _moveVisualizer, _gridService, _turnCalculationsService,
            _commandHistoryService, _currentTurnViewer, _finishService, PlayerId.Player2, color2);
        
        SetActivePlayer(Random.value > 0.5f ? _player1 : _player2);
    }

    private void SwitchPlayers() => 
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
    void EndTurn();
}
