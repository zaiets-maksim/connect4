using System;
using Connect4.Scripts.Services.VictoryCheckerService;
using Infrastructure.Services.Factories.Game;
using Infrastructure.Services.Factories.UIFactory;
using Zenject;

namespace Infrastructure.StateMachine.Game.States
{
    public class LoadLevelState : IPayloadedState<Tuple<Player, Player>>, IGameState
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
        private readonly IVictoryVisualizer _victoryVisualizer;
        private Tuple<Player, Player> _players;

        [Inject]
        public LoadLevelState(IStateMachine<IGameState> gameStateMachine, ISceneLoader sceneLoader,
            ILoadingCurtain loadingCurtain, IUIFactory uiFactory, IGameFactory gameFactory, IGameCurator gameCurator,
            IMoveVisualizer moveVisualizer, IGridService gridService, IVictoryCheckerService victoryCheckerService,
            ITurnCalculationsService turnCalculationsService, ICommandHistoryService commandHistoryService,
            IVictoryVisualizer victoryVisualizer)
        {
            _victoryVisualizer = victoryVisualizer;
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

        public void Enter(Tuple<Player, Player> players)
        {
            _players = players;
            _loadingCurtain.Show();
            _sceneLoader.Load("First", OnLevelLoad);
        }

        public void Exit()
        {
            _loadingCurtain.Hide();
        }

        protected virtual void OnLevelLoad()
        {
            Clear();
            InitGameWorld();

            _gameStateMachine.Enter<GameLoopState>();
        }

        private void InitGameWorld()
        { 
           _gameFactory.CreateCamera();
           _uiFactory.CreateUiRoot();
           _gameFactory.CreateGrid();
           _gameFactory.CreateHud();

           _gameCurator.SetPlayers(_players.Item1, _players.Item2);
        }

        private void Clear()
        {
            _gridService.Clear();
        }
    }
}