using Connect4.Scripts.Infrastructure;
using Connect4.Scripts.Infrastructure.StateMachine.Game.States;
using Connect4.Scripts.Services.CommandHistoryService;
using Connect4.Scripts.Services.Factories.Game;
using Connect4.Scripts.Services.Factories.UIFactory;
using Connect4.Scripts.Services.FinishService;
using Connect4.Scripts.Services.GameCurator;
using Connect4.Scripts.Services.GridService;
using Connect4.Scripts.Services.MoveVisualizer;
using Connect4.Scripts.Services.StaticData;
using Connect4.Scripts.Services.TurnCalculationsService;
using Connect4.Scripts.Services.VictoryCheckerService;
using Connect4.Scripts.Services.VictoryVisualizer;
using Infrastructure.StateMachine;
using Infrastructure.StateMachine.Game;
using Infrastructure.StateMachine.Game.States;
using UnityEngine;
using Zenject;
using Application = UnityEngine.Application;

namespace Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private CoroutineRunner _coroutineRunner;
        [SerializeField] private LoadingCurtain _curtain;

        private RuntimePlatform Platform => Application.platform;

        public override void InstallBindings()
        {
            Debug.Log("Installer");

            BindMonoServices();
            BindServices();

            BindGameStateMachine();
            BindGameStates();

            BootstrapGame(); 
        }

        private void BindServices()
        {
            BindStaticDataService();
            Container.BindInterfacesTo<UIFactory>().AsSingle();
            Container.BindInterfacesTo<GameFactory>().AsSingle();
            Container.BindInterfacesTo<GridService>().AsSingle();
            Container.BindInterfacesTo<GameCurator>().AsSingle();
            Container.BindInterfacesTo<MoveVisualizer>().AsSingle();
            Container.BindInterfacesTo<VictoryCheckerService>().AsSingle();
            Container.BindInterfacesTo<TurnCalculationsService>().AsSingle();
            Container.BindInterfacesTo<CommandHistoryService>().AsSingle();
            Container.BindInterfacesTo<VictoryVisualizer>().AsSingle();
            Container.BindInterfacesTo<FinishService>().AsSingle();
        }

        private void BindMonoServices()
        {
            Container.Bind<ICoroutineRunner>().FromMethod(() => Container.InstantiatePrefabForComponent<ICoroutineRunner>(_coroutineRunner)).AsSingle();
            Container.Bind<ILoadingCurtain>().FromMethod(() => Container.InstantiatePrefabForComponent<ILoadingCurtain>(_curtain)).AsSingle();

            BindSceneLoader();
        }

        private void BindSceneLoader()
        {
            ISceneLoader sceneLoader = new SceneLoader(Container.Resolve<ICoroutineRunner>());
            Container.Bind<ISceneLoader>().FromInstance(sceneLoader).AsSingle();
        }

        private void BindStaticDataService()
        {
            IStaticDataService staticDataService = new StaticDataService();
            staticDataService.LoadData();
            Container.Bind<IStaticDataService>().FromInstance(staticDataService).AsSingle();
        }

        private void BindGameStateMachine()
        {
            Container.Bind<GameStateFactory>().AsSingle();
            Container.BindInterfacesTo<GameStateMachine>().AsSingle();
        }

        private void BindGameStates()
        {
            Container.Bind<BootstrapState>().AsSingle();
            Container.Bind<LoadProgressState>().AsSingle();
            Container.Bind<LoadLevelState>().AsSingle();
            Container.Bind<MenuLevelState>().AsSingle();
            Container.Bind<GameLoopState>().AsSingle();
        }

        private void BootstrapGame() => 
            Container.Resolve<IStateMachine<IGameState>>().Enter<BootstrapState>();
    }
}