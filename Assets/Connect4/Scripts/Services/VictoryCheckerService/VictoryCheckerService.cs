using Connect4.Scripts.IWinningStrategy;
using UnityEngine;

namespace Connect4.Scripts.Services.VictoryCheckerService
{
    public class VictoryCheckerService : IVictoryCheckerService
    {
        private readonly VerticalWinStrategy _verticalWinStrategy = new VerticalWinStrategy();
        private readonly HorizontalWinStrategy _horizontalWinStrategy = new HorizontalWinStrategy();
        private readonly DiagonalWinStrategy _diagonalWinStrategy = new DiagonalWinStrategy();
    
        private readonly IGridService _gridService;

        public VictoryCheckerService(IGridService gridService) => 
            _gridService = gridService;

        public bool TurnWasWin(Vector2Int index, PlayerId playerId) =>
            _verticalWinStrategy.IsWinningMove(_gridService, index, playerId) ||
            _horizontalWinStrategy.IsWinningMove(_gridService, index, playerId) ||
            _diagonalWinStrategy.IsWinningMove(_gridService, index, playerId);
    }

    public interface IVictoryCheckerService
    {
        bool TurnWasWin(Vector2Int index, PlayerId playerId);
    }
}