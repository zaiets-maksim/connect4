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
    protected CurrentTurnViewer _currentTurnViewer;
    protected IVictoryVisualizer _victoryVisualizer;

    public Color Color { get; private set; }
    public PlayerId PlayerId { get; private set; }
    public bool IsReady { get; set; }

    public void Initialize(IGameCurator gameCurator, IMoveVisualizer moveVisualizer, IGridService gridService,
        IVictoryCheckerService victoryCheckerService, ITurnCalculationsService turnCalculationsService, 
        ICommandHistoryService commandHistoryService, CurrentTurnViewer currentTurnViewer, IVictoryVisualizer victoryVisualizer,
        PlayerId playerId, Color color)
    {
        Color = color;
        PlayerId = playerId;
        _victoryVisualizer = victoryVisualizer;
        _currentTurnViewer = currentTurnViewer;
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
        Debug.Log(_currentTurnViewer);
        _currentTurnViewer.UpdateTurn($"{PlayerId} turn", Color);
        Debug.Log($"<color=yellow>{PlayerId} ({PlayerId.GetType()}) </color> awaiting...");

        if (!IsHuman()) 
            ComputerTurn();
    }

    private void ComputerTurn()
    {
        Vector2Int index = _turnCalculationsService.GetBestDecisionFor(this);
        DoTurn(index);
        Debug.Log($"Best decision: {index}");
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
        
        if (_victoryCheckerService.TurnIsWin(index, PlayerId)) // to finish service && check full field
        {
            _currentTurnViewer.UpdateTurn($"{PlayerId} win!", Color);
            _victoryVisualizer.Show(_victoryCheckerService.GetWinStrategies(), Color);
            
            Debug.Log($"<color=green>{PlayerId} </color> win!");
            return;
        }
        
        _gameCurator.EndTurn();
    }
}

public class Computer : Player
{
    private Vector2Int _delay = new Vector2Int(100, 300);
    
    public override async Task DoTurn(Vector2Int index)
    {
        var delay = Random.Range(_delay.x, _delay.y);
        await Task.Delay(delay);
        
        _gameCurator.ActivePlayer.IsReady = false;

        MoveCommand moveCommand = new MoveCommand(index, this, _commandHistoryService, _gridService, _moveVisualizer);
        await moveCommand.Execute();
        moveCommand.ToHistory();

        if (_victoryCheckerService.TurnIsWin(index, PlayerId)) // to finish service && check full field
        {
            _currentTurnViewer.UpdateTurn($"{PlayerId} win!", Color);
            _victoryVisualizer.Show(_victoryCheckerService.GetWinStrategies(), Color);
            Debug.Log($"<color=green>{PlayerId} </color> win!");
            return;
        }

        _gameCurator.EndTurn();
    }
}