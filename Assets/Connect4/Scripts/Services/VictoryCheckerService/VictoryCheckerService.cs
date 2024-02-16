using System.Collections.Generic;
using System.Linq;
using Connect4.Scripts.Strategies.Win;
using UnityEngine;

namespace Connect4.Scripts.Services.VictoryCheckerService
{
    public class VictoryCheckerService : IVictoryCheckerService
    {
        private readonly VerticalWinStrategy _verticalWinStrategy = new VerticalWinStrategy();
        private readonly HorizontalWinStrategy _horizontalWinStrategy = new HorizontalWinStrategy();
        private readonly DiagonalWinStrategy _diagonalWinStrategy = new DiagonalWinStrategy();

        private readonly IGridService _gridService;
        private List<IWinningStrategy> _wonStrategies;
        
        
        public VictoryCheckerService(IGridService gridService) =>
            _gridService = gridService;

        public bool TurnIsWin(Vector2Int index, PlayerId playerId)
        {
            // Debug.Log(index);
            // var indexes = _gridService.GetRow(5).Select(x => x.CellId).ToList();
            // string listString = string.Join(" ", indexes);
            // Debug.Log(listString);

            var strategies = new IWinningStrategy[] 
            {
                _verticalWinStrategy,
                _horizontalWinStrategy,
                _diagonalWinStrategy
            };

            _wonStrategies = strategies
                .Where(strategy => strategy.IsWinningMove(_gridService, index, playerId))
                .ToList();

            return _wonStrategies.Count > 0;
        }


        public bool TurnIsWin(Vector2Int index, PlayerId playerId, out int counter)
        {
            counter = 0;

            counter += _verticalWinStrategy.IsWinningMove(_gridService, index, playerId) ? 1 : 0;
            counter += _horizontalWinStrategy.IsWinningMove(_gridService, index, playerId) ? 1 : 0;
            counter += _diagonalWinStrategy.IsWinningMove(_gridService, index, playerId) ? 1 : 0;

            return counter > 0;
        }

        public List<IWinningStrategy> GetWinStrategies() => _wonStrategies;
    }

    public interface IVictoryCheckerService
    {
        bool TurnIsWin(Vector2Int index, PlayerId playerId);
        bool TurnIsWin(Vector2Int index, PlayerId playerId, out int counter);
        List<IWinningStrategy> GetWinStrategies();
    }
}