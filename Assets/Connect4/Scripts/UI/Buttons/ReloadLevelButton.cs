using Infrastructure.StateMachine;
using Infrastructure.StateMachine.Game.States;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Connect4.Scripts.UI.Buttons
{
    public class ReloadLevelButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
    
        private IStateMachine<IGameState> _gameStateMachine;

        [Inject]
        public void Constructor(IStateMachine<IGameState> gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        private void Start() => _button.onClick.AddListener(Reload);

        private void Reload() => _gameStateMachine.Back();
    }
}
