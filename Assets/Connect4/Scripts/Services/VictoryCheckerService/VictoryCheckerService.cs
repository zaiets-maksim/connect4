using System.Collections.Generic;
using System.Linq;
using Connect4.Scripts.Field;
using Connect4.Scripts.Services.GridService;
using Connect4.Scripts.Strategies.Win;
using UnityEngine;

namespace Connect4.Scripts.Services.VictoryCheckerService
{
    public class VictoryCheckerService : IVictoryCheckerService
    {
        private readonly VerticalWinStrategy _verticalWinStrategy;
        private readonly HorizontalWinStrategy _horizontalWinStrategy;
        private readonly DiagonalWinStrategy _diagonalWinStrategy;

        private readonly IGridService _gridService;
        private List<IWinningStrategy> _wonStrategies;

        public VictoryCheckerService(IGridService gridService)
        {
            _gridService = gridService;
            
            _verticalWinStrategy = new VerticalWinStrategy(_gridService);
            _horizontalWinStrategy = new HorizontalWinStrategy(_gridService);
            _diagonalWinStrategy = new DiagonalWinStrategy(_gridService);
        }

        public bool TurnIsWin(Vector2Int index, PlayerId playerId)
        {
            var strategies = new IWinningStrategy[] 
            {
                _verticalWinStrategy,
                _horizontalWinStrategy,
                _diagonalWinStrategy
            };

            _wonStrategies = strategies
                .Where(strategy => strategy.IsWinningMove(index, playerId))
                .ToList();

            return _wonStrategies.Count > 0;
        }


        public bool TurnIsWin(Vector2Int index, PlayerId playerId, out int counter)
        {
            counter = 0;

            counter += _verticalWinStrategy.IsWinningMove(index, playerId) ? 1 : 0;
            counter += _horizontalWinStrategy.IsWinningMove(index, playerId) ? 1 : 0;
            counter += _diagonalWinStrategy.IsWinningMove(index, playerId) ? 1 : 0;

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