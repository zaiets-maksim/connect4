using System.Threading.Tasks;
using Connect4.Scripts.Commands;
using Connect4.Scripts.Services.VictoryCheckerService;
using UnityEngine;

public abstract class Player
{
    protected IGameCurator _gameCurator;
    protected IMoveVisualizer _moveVisualizer;
    protected IGridService _gridService;
    protected IVictoryCheckerService _victoryCheckerService;
    protected ITurnCalculationsService _turnCalculationsService;
    protected ICommandHistoryService _commandHistoryService;

    public Color Color { get; private set; }
    public PlayerId PlayerId { get; private set; }
    public bool IsReady { get; set; }

    public void Initialize(IGameCurator gameCurator, IMoveVisualizer moveVisualizer, IGridService gridService,
        IVictoryCheckerService victoryCheckerService, ITurnCalculationsService turnCalculationsService, 
        ICommandHistoryService commandHistoryService, PlayerId playerId, Color color)
    {
        Color = color;
        PlayerId = playerId;
        _turnCalculationsService = turnCalculationsService;
        _victoryCheckerService = victoryCheckerService;
        _gridService = gridService;
        _moveVisualizer = moveVisualizer;
        _gameCurator = gameCurator;
        _commandHistoryService = commandHistoryService;
    }
    
    public bool IsHuman() => this is Human;

    public abstract Task DoTurn(Vector2Int index);

    public void AwaitTurn()
    {
        _gameCurator.ActivePlayer.IsReady = true;
        
        Debug.Log('\n');
        Debug.Log($"<color=yellow>{_gameCurator.ActivePlayer.PlayerId} ({_gameCurator.ActivePlayer.GetType()}) </color> awaiting...");
        
        if (!IsHuman()) 
            ComputerTurn();
    }

    private void ComputerTurn()
    {
        Vector2Int index = _turnCalculationsService.GetBestDecision();
        Debug.Log($"Best decision: {index}");
        DoTurn(index);
    }
}

public class Human : Player
{
    public override async Task DoTurn(Vector2Int index)
    {
        _gameCurator.ActivePlayer.IsReady = false;

        MoveCommand moveCommand = new MoveCommand(index, this, _commandHistoryService, _gridService, _moveVisualizer);
        await moveCommand.Execute();
        moveCommand.ToHistory();
        
        if (_victoryCheckerService.TurnIsWin(index, _gameCurator.ActivePlayer.PlayerId)) // to finish service && check full field
            return;
        
        _gameCurator.EndTurn();
    }
}

public class Computer : Player
{
    public override async Task DoTurn(Vector2Int index)
    {
        var delay = Random.Range(100, 300);
        await Task.Delay(delay);
        
        _gameCurator.ActivePlayer.IsReady = false;

        MoveCommand moveCommand = new MoveCommand(index, this, _commandHistoryService, _gridService, _moveVisualizer);
        await moveCommand.Execute();
        moveCommand.ToHistory();
        
        _gameCurator.EndTurn();
    }
}