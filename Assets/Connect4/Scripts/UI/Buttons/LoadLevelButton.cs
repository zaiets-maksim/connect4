using System;
using Connect4.Scripts.Player;
using Connect4.Scripts.Services.GameCurator;
using Infrastructure.StateMachine;
using Infrastructure.StateMachine.Game.States;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Connect4.Scripts.UI.Buttons
{
    public class LoadLevelButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameMode _gameMode;

        private IStateMachine<IGameState> _stateMachine;
        private IGameCurator _gameCurator;

        [Inject]
        public void Constructor(IStateMachine<IGameState> stateMachine, IGameCurator gameCurator)
        {
            _gameCurator = gameCurator;
            _stateMachine = stateMachine;
        }

        private void Start() => _button.onClick.AddListener(LoadLevel);

        private void LoadLevel()
        {
            Tuple<Player.Player, Player.Player> players = _gameMode switch
            {
                GameMode.HumanVsHuman => new Tuple<Player.Player, Player.Player>(new Human(), new Human()),
                GameMode.HumanVsComputer => new Tuple<Player.Player, Player.Player>(new Human(), new Computer()),
                GameMode.ComputerVsComputer => new Tuple<Player.Player, Player.Player>(new Computer(), new Computer()),
                _ => null
            };

            _stateMachine.Enter<LoadLevelState, Tuple<Player.Player, Player.Player>>(players);
            _gameCurator.Init(_gameMode);
        }
    }
}

public enum GameMode
{
    HumanVsHuman,
    HumanVsComputer,
    ComputerVsComputer
}

