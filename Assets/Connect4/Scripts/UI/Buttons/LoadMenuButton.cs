using Infrastructure.StateMachine;
using Infrastructure.StateMachine.Game.States;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Connect4.Scripts.UI.Buttons
{
    public class LoadMenuButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private IStateMachine<IGameState> _stateMachine;

        [Inject]
        public void Constructor(IStateMachine<IGameState> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        private void Start()
        {
            _button.onClick.AddListener(ToMenu);
        }

        private void ToMenu() => 
            _stateMachine.Enter<MenuLevelState, string>("Menu");
    }
}