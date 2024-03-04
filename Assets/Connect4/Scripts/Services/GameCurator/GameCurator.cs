using System;
using Connect4.Scripts.Field;
using Connect4.Scripts.Services.CommandHistoryService;
using Connect4.Scripts.Services.Factories.Game;
using Connect4.Scripts.Services.FinishService;
using Connect4.Scripts.Services.GridService;
using Connect4.Scripts.Services.MoveVisualizer;
using Connect4.Scripts.Services.TurnCalculationsService;
using Connect4.Scripts.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Connect4.Scripts.Services.GameCurator
{
    public class GameCurator : IGameCurator
    {
        private Player.Player _player1, _player2;
        private readonly IMoveVisualizer _moveVisualizer;
        private readonly IGridService _gridService;
        private readonly ITurnCalculationsService _turnCalculationsService;
        private readonly ICommandHistoryService _commandHistoryService;
        private readonly IGameFactory _gameFactory;
        private readonly IFinishService _finishService;

        private CurrentTurnViewer _currentTurnViewer => _gameFactory.Hud.CurrentTurnViewer;

        public Player.Player ActivePlayer { get; private set; }
        public GameMode GameMode { get; private set; }

        public event Action OnSwitchPlayers;

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

        public void SetPlayers<T>(T player1, T player2) where T : Player.Player
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
            
            BindToColumnClick();

            SetActivePlayer(Random.value > 0.5f ? _player1 : _player2);
        }
    
        public void EndTurn() => SwitchPlayers();
    
        public void Init(GameMode gameMode) => GameMode = gameMode;
        
        private void SetActivePlayer(Player.Player player)
        {
            _player1.IsReady = false;
            _player2.IsReady = false;

            ActivePlayer = player;
        
            ActivePlayer.AwaitTurn();
        }
        
        private void SwitchPlayers()
        {
            SetActivePlayer(ActivePlayer == _player1 ? _player2 : _player1);
            OnSwitchPlayers?.Invoke();
        }
        
        private void BindToColumnClick()
        {
            foreach (var column in _gridService.Columns) 
                column.OnClick += ProcessHumanTurn;
        }

        private void ProcessHumanTurn(Vector2Int index)
        {
            if (ActivePlayer.IsHuman()) 
                ActivePlayer.DoTurn(index);
        }
    }

    public interface IGameCurator
    {
        event Action OnSwitchPlayers;
        Player.Player ActivePlayer { get; }
        GameMode GameMode { get; }
        void SetPlayers<T>(T player1, T player2) where T : Player.Player;
        void EndTurn();
        void Init(GameMode gameMode);
    }
}