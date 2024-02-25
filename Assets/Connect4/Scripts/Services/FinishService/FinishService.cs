using Connect4.Scripts.Services.GridService;
using Connect4.Scripts.Services.VictoryCheckerService;
using Connect4.Scripts.Services.VictoryVisualizer;
using Connect4.Scripts.UI;
using UnityEngine;

namespace Connect4.Scripts.Services.FinishService
{
    public class FinishService : IFinishService
    {
        private IVictoryCheckerService _victoryCheckerService;
        private IVictoryVisualizer _victoryVisualizer;
        private IGridService _gridService;
        private CurrentTurnViewer _currentTurnViewer;

        public void Initialize(IVictoryCheckerService victoryCheckerService, IVictoryVisualizer victoryVisualizer,
            IGridService gridService, CurrentTurnViewer currentTurnViewer)
        {
            _currentTurnViewer = currentTurnViewer;
            _gridService = gridService;
            _victoryVisualizer = victoryVisualizer;
            _victoryCheckerService = victoryCheckerService;
        }
    
        public bool CheckFinish(Vector2Int index, Player.Player player)
        {
            if (_victoryCheckerService.TurnIsWin(index, player.PlayerId)) // to finish service && check full field
            {
                _currentTurnViewer.UpdateTurn($"{player.PlayerId} win!", player.Color);
                _victoryVisualizer.Show(_victoryCheckerService.GetWinStrategies(), player.Color);
            
                Debug.Log($"<color=green>{player.PlayerId} </color> win!");
                return true;
            }
        
        
            if (!_gridService.GridHasFreeCell())
            {
                _currentTurnViewer.UpdateTurn("Draw!", Color.white);
                return true;
            }

            return false;
        }
    
    }

    public interface IFinishService
    {
        void Initialize(IVictoryCheckerService victoryCheckerService, IVictoryVisualizer victoryVisualizer,
            IGridService gridService, CurrentTurnViewer currentTurnViewer);

        bool CheckFinish(Vector2Int index, Player.Player player);
    }
}