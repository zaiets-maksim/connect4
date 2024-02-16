using Infrastructure.StateMachine;
using Infrastructure.StateMachine.Game.States;

namespace Connect4.Scripts.Infrastructure.StateMachine.Game.States
{
    public class LoadProgressState : IPayloadedState<string>, IGameState
    {
        private readonly IStateMachine<IGameState> _stateMachine;

        public LoadProgressState(IStateMachine<IGameState> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter(string payload)
        {
            // _stateMachine.Enter<LoadLevelState, string>(payload);
        }

        public void Exit()
        {
            
        }
    }
}