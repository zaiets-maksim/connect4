using System.Threading.Tasks;
using Connect4.Scripts.Commands;
using Connect4.Scripts.Field;
using Connect4.Scripts.Services.CommandHistoryService;
using Connect4.Scripts.Services.FinishService;
using Connect4.Scripts.Services.GameCurator;
using Connect4.Scripts.Services.GridService;
using Connect4.Scripts.Services.MoveVisualizer;
using Connect4.Scripts.Services.TurnCalculationsService;
using Connect4.Scripts.UI;
using UnityEngine;

namespace Connect4.Scripts.Player
{
    public abstract class Player
    {
        protected IGameCurator _gameCurator;
        protected IMoveVisualizer _moveVisualizer;
        protected IGridService _gridService;
        protected ITurnCalculationsService _turnCalculationsService;
        protected ICommandHistoryService _commandHistoryService;
        protected CurrentTurnViewer _currentTurnViewer;
        protected IFinishService _finishService;

        public Color Color { get; private set; }
        public PlayerId PlayerId { get; private set; }
        public bool IsReady { get; set; }

        public void Initialize(IGameCurator gameCurator, IMoveVisualizer moveVisualizer, IGridService gridService,
            ITurnCalculationsService turnCalculationsService, ICommandHistoryService commandHistoryService,
            CurrentTurnViewer currentTurnViewer, IFinishService finishService, PlayerId playerId, Color color)
        {
            Color = color;
            PlayerId = playerId;
            _finishService = finishService;
            _currentTurnViewer = currentTurnViewer;
            _turnCalculationsService = turnCalculationsService;
            _gridService = gridService;
            _moveVisualizer = moveVisualizer;
            _gameCurator = gameCurator;
            _commandHistoryService = commandHistoryService;
        }
    
        public abstract bool IsHuman();

        public abstract Task DoTurn(Vector2Int index);

        public void AwaitTurn()
        {
            _gameCurator.ActivePlayer.IsReady = true;
        
            Debug.Log('\n');
            _currentTurnViewer.UpdateTurn($"{PlayerId} turn", Color);
            Debug.Log($"<color=yellow>{PlayerId} ({GetType().Name}) </color> awaiting...");

            if (!IsHuman()) 
                ComputerTurn();
        }

        private void ComputerTurn()
        {
            Vector2Int index = _turnCalculationsService.GetBestDecisionFor(this);
            index = _gridService.TakeColumn(index.y);
            DoTurn(index);
            Debug.Log($"Best decision: {index}");
        }
    }

    public class Human : Player
    {
        public override bool IsHuman() => true;

        public override async Task DoTurn(Vector2Int index)
        {
            _gameCurator.ActivePlayer.IsReady = false;

            MoveCommand moveCommand = new MoveCommand(index, this, _commandHistoryService, _gridService, _moveVisualizer);
            await moveCommand.Execute();
            moveCommand.ToHistory();
        
            if (_finishService.CheckFinish(index, this))
                return;

            _gameCurator.EndTurn();
        }
    }

    public class Computer : Player
    {
        public override bool IsHuman() => false;
        
        private Vector2Int _delay = new Vector2Int(100, 300);
    
        public override async Task DoTurn(Vector2Int index)
        {
            var delay = Random.Range(_delay.x, _delay.y);
            await Task.Delay(delay);
        
            _gameCurator.ActivePlayer.IsReady = false;

            MoveCommand moveCommand = new MoveCommand(index, this, _commandHistoryService, _gridService, _moveVisualizer);
            await moveCommand.Execute();
            moveCommand.ToHistory();

            if (_finishService.CheckFinish(index, this))
                return;

            _gameCurator.EndTurn();
        }
    }
}