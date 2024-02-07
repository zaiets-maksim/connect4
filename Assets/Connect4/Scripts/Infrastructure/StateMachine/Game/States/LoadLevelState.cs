using Connect4.Scripts.Services.VictoryCheckerService;
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
        private IGameCurator _gameCurator;
        private IGameFactory _gameFactory;
        private IMoveVisualizer _moveVisualizer;
        private IGridService _gridService;
        private IVictoryCheckerService _victoryCheckerService;

        [Inject]
        public LoadLevelState(IStateMachine<IGameState> gameStateMachine, ISceneLoader sceneLoader,
            ILoadingCurtain loadingCurtain, IUIFactory uiFactory, IGameFactory gameFactory, IGameCurator gameCurator,
            IMoveVisualizer moveVisualizer, IGridService gridService, IVictoryCheckerService victoryCheckerService)
        {
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
           _gameCurator.Initialize(_moveVisualizer, _gridService, _victoryCheckerService);
           _gameCurator.SetPlayers<Player>(new Human(), new Human());
        }
    }
}