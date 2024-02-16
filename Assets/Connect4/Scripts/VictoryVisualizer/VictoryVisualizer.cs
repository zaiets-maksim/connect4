using System.Collections.Generic;
using System.Threading.Tasks;
using Connect4.Scripts.Strategies.Win;
using Infrastructure.Services.Factories.Game;
using UnityEngine;

public class VictoryVisualizer : IVictoryVisualizer
{
    private readonly IGridService _gridService;
    private readonly IGameFactory _gameFactory;


    public VictoryVisualizer(IGridService gridService, IGameFactory gameFactory)
    {
        _gameFactory = gameFactory;
        _gridService = gridService;
    }

    public async void Show(List<IWinningStrategy> wonStrategies, Color color)
    {
        if (wonStrategies.Count > 0)
        {
            var indexes = wonStrategies[Random.Range(0, wonStrategies.Count)].FourInLine;

            foreach (var index in indexes)
            {
                await Task.Delay(50);

                var cell = _gridService.GetCell(index);
                var shine = _gameFactory.CreateShine();
                shine.Play(cell.Position, color);
            }
        }
    }
}

public interface IVictoryVisualizer
{
    void Show(List<IWinningStrategy> getWinStrategies, Color color);
}
