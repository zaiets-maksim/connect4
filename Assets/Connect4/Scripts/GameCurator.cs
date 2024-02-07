using System.Threading.Tasks;
using Connect4.Scripts.IWinningStrategy;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameCurator : IGameCurator
{
    private Player _player1, _player2;
    private IMoveVisualizer _moveVisualizer;
    private IGridService _gridService;

    public Player ActivePlayer { get; private set; }

    public void Initialize(IMoveVisualizer moveVisualizer, IGridService gridService)
    {
        _gridService = gridService;
        _moveVisualizer = moveVisualizer;
    }

    public void Start()
    {
        SetPlayers<Player>(new Human(), new Human());
        SetPlayers<Player>(new Human(), new Computer());
        SetPlayers<Player>(new Computer(), new Computer());
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

        _player1.Initialize(this, _moveVisualizer, _gridService, PlayerId.Player1, color1);
        _player2.Initialize(this, _moveVisualizer, _gridService, PlayerId.Player2, color2);
        
        SetActivePlayer(Random.value > 0.5f ? _player1 : _player2);
    }

    public void SwitchPlayers() => 
        SetActivePlayer(ActivePlayer == _player1 ? _player2 : _player1);

    public void EndTurnHuman()
    {
        SwitchPlayers();        
    }

    public void EndTurnComputer()
    {
        SwitchPlayers();
    }

    public async void WaitForHumanTurn()
    {
        // run Timer
    }

    public async void WaitForComputerTurn()
    {
        // run calculations
    }
}

public interface IGameCurator
{
    Player ActivePlayer { get; }
    void SetPlayers<T>(T player1, T player2) where T : Player;
    void SwitchPlayers();
    void EndTurnHuman();
    void EndTurnComputer();
    void WaitForHumanTurn();
    void WaitForComputerTurn();
    void Initialize(IMoveVisualizer moveVisualizer, IGridService gridService);
}

public abstract class Player
{
    protected IGameCurator _gameCurator;
    protected IMoveVisualizer _moveVisualizer;
    protected IGridService _gridService;
    public Color Color { get; private set; }
    public PlayerId PlayerId { get; private set; }
    public bool IsReady { get; set; }

    public void Initialize(IGameCurator gameCurator, IMoveVisualizer moveVisualizer, IGridService gridService,
        PlayerId playerId, Color color)
    {
        Color = color;
        PlayerId = playerId;
        _gridService = gridService;
        _moveVisualizer = moveVisualizer;
        _gameCurator = gameCurator;
    }
    
    public abstract Task DoTurn(Vector2Int index);

    public void AwaitTurn()
    {
        _gameCurator.ActivePlayer.IsReady = true;
        Debug.Log('\n');
        Debug.Log($"<color=yellow>{_gameCurator.ActivePlayer.PlayerId} </color> awaiting...");
    }
}

public class Human : Player
{
    public override async Task DoTurn(Vector2Int index)
    {
        _gameCurator.ActivePlayer.IsReady = false;

        Debug.Log($"{_gameCurator.ActivePlayer.PlayerId} is making a move");
        await _moveVisualizer.ShowTurn(_gridService.GetCell(index.x, index.y).Position);
        _gridService.TakeCell(index.x, index.y, _gameCurator.ActivePlayer.PlayerId);
        
        Debug.Log($"{_gameCurator.ActivePlayer.PlayerId} made a move");
        
        VerticalWinStrategy verticalWinStrategy = new VerticalWinStrategy();
        HorizontalWinStrategy horizontalWinStrategy = new HorizontalWinStrategy();
        DiagonalWinStrategy diagonalWinStrategy = new DiagonalWinStrategy();
        
        // if(verticalWinStrategy.IsWinningMove(_gridService, index, _gameCurator.ActivePlayer.PlayerId))
        //     return;
        
        if(horizontalWinStrategy.IsWinningMove(_gridService, index, _gameCurator.ActivePlayer.PlayerId))
            return;
        
        // if(diagonalWinStrategy.IsWinningMove(_gridService, index, _gameCurator.ActivePlayer.PlayerId)) 
        //     return;

        _gameCurator.EndTurnHuman();
    }
}

public class Computer : Player
{

    public override async Task DoTurn(Vector2Int index)
    {
        
    }
}
