using Connect4.Scripts.Services.CommandHistoryService;
using Connect4.Scripts.Services.GameCurator;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Connect4.Scripts.UI.Buttons
{
    public class UndoButton : MonoBehaviour
    {
        [SerializeField] Button _button;
        [SerializeField] Image _image;
        [SerializeField] Text _text;
    
        private ICommandHistoryService _commandHistoryService;
        private IGameCurator _gameCurator;

        [Inject]
        public void Constructor(ICommandHistoryService commandHistoryService, IGameCurator gameCurator)
        {
            _gameCurator = gameCurator;
            _commandHistoryService = commandHistoryService;
        }

        private void Start()
        {
            Hide();
            _button.onClick.AddListener(TryUndo);

            if (_gameCurator.GameMode == GameMode.HumanVsComputer) 
                _gameCurator.OnSwitchPlayers += TryShowButton;
        }

        private void OnDestroy()
        {
            if (_gameCurator.GameMode == GameMode.HumanVsComputer) 
                _gameCurator.OnSwitchPlayers -= TryShowButton;
        }

        private void TryShowButton()
        {
            if (_commandHistoryService.CommandsCount() > 1)
            {
                if (_gameCurator.ActivePlayer.IsHuman())
                    Show();
                else
                    Hide();
            }
        }

        private void Show()
        {
            _image.gameObject.SetActive(true);
            _text.gameObject.SetActive(true);
        }

        private void Hide()
        {
            _image.gameObject.SetActive(false);
            _text.gameObject.SetActive(false);
        }

        private void TryUndo()
        {
            if (_commandHistoryService.HasCommands()) 
                Undo();
        }
    
        private void Undo()
        {
            var opponentCommand = _commandHistoryService.Pop();
            opponentCommand.Undo();
        
            var myCommand = _commandHistoryService.Pop();
            myCommand.Undo();

            Hide();
        }
    }
}
