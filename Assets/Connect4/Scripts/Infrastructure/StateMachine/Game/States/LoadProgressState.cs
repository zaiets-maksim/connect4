using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Services.PersistenceProgress;
using UnityEngine;
using Zenject;

namespace Infrastructure.StateMachine.Game.States
{
    public class LoadProgressState : IPayloadedState<string>, IGameState
    {
        private readonly IStateMachine<IGameState> _stateMachine;
        private readonly IPersistenceProgressService _progressService;
        private readonly ISceneLoader _sceneLoader;

        public LoadProgressState(IStateMachine<IGameState> stateMachine, IPersistenceProgressService progressService, ISceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _progressService = progressService;
            _sceneLoader = sceneLoader;
        }

        public void Enter(string payload)
        {
            LoadOrCreatePlayerData();
            _stateMachine.Enter<LoadLevelState, string>(payload);

        }

        public void Exit()
        {
            
        }

        private PlayerData LoadOrCreatePlayerData() => 
            _progressService.PlayerData = new PlayerData();
    }

    public interface IFPSMeter
    {
        void Begin();
        void Break();
    }

    public class FPSMeter: IFPSMeter
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IPersistenceProgressService _progressService;
        
        private Queue<float> _capturedFrames = new Queue<float>();
        private int _framesCount = 10;
        private Coroutine _coroutine;

        [Inject]
        public FPSMeter(ICoroutineRunner coroutineRunner,IPersistenceProgressService progressService)
        {
            _coroutineRunner = coroutineRunner;
            _progressService = progressService;
        }

        private float AverageFPS => _capturedFrames.Average();
        

        public void Begin()
        {
            _coroutine = _coroutineRunner.StartCoroutine(LoopFPSCheck());
        }

        public void Break()
        {
            _coroutineRunner.StopCoroutine(_coroutine);
        }
        
        private IEnumerator LoopFPSCheck()  
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                CaptureFrame();
            }
        }

        private void CaptureFrame()
        {
            if (NeedDequeue())
                _capturedFrames.Dequeue();

            _capturedFrames.Enqueue(CurrentFPS());
        }

        private bool NeedDequeue() => 
            _capturedFrames.Count > _framesCount;

        private float CurrentFPS() => 
            1 / Time.deltaTime;
    }
}