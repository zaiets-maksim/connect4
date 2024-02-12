﻿using Connect4.Scripts.Services.VictoryCheckerService;
using Infrastructure.Services.Factories.Game;
using Infrastructure.Services.Factories.UIFactory;
using Zenject;

namespace Infrastructure.StateMachine.Game.States
{
    public class LoadLevelState : IPayloadedState<string>, IGameState
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly ILoadingCurtain _loadingCurtain;
        private readonly IUIFactory _uiFactory;
        private readonly IStateMachine<IGameState> _gameStateMachine;
        private readonly IGameCurator _gameCurator;
        private readonly IGameFactory _gameFactory;
        private readonly IMoveVisualizer _moveVisualizer;
        private readonly IGridService _gridService;
        private readonly IVictoryCheckerService _victoryCheckerService;
        private readonly ITurnCalculationsService _turnCalculationsService;
        private readonly ICommandHistoryService _commandHistoryService;

        [Inject]
        public LoadLevelState(IStateMachine<IGameState> gameStateMachine, ISceneLoader sceneLoader,
            ILoadingCurtain loadingCurtain, IUIFactory uiFactory, IGameFactory gameFactory, IGameCurator gameCurator,
            IMoveVisualizer moveVisualizer, IGridService gridService, IVictoryCheckerService victoryCheckerService,
            ITurnCalculationsService turnCalculationsService, ICommandHistoryService commandHistoryService)
        {
            _commandHistoryService = commandHistoryService;
            _turnCalculationsService = turnCalculationsService;
            _victoryCheckerService = victoryCheckerService;
            _gridService = gridService;
            _moveVisualizer = moveVisualizer;
            _gameCurator = gameCurator;
            _gameFactory = gameFactory;
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _uiFactory = uiFactory;
        }

        public void Enter(string payload)
        {
            _loadingCurtain.Show();
            _sceneLoader.Load(payload, OnLevelLoad);
        }

        public void Exit()
        {
            _loadingCurtain.Hide();
        }

        protected virtual void OnLevelLoad()
        {
            InitGameWorld();

            _gameStateMachine.Enter<GameLoopState>();
        }

        private void InitGameWorld()
        {
           _uiFactory.CreateUiRoot();
           _gameFactory.CreateGrid();
           _gameCurator.Initialize(_moveVisualizer, _gridService, _victoryCheckerService, _turnCalculationsService, _commandHistoryService);
           _gameCurator.SetPlayers<Player>(new Human(), new Computer());
        }
    }
}