using System.Collections.Generic;
using Connect4.Scripts.Commands;
using Connect4.Scripts.Turns;

namespace Connect4.Scripts.Strategies.Build
{
    public interface ILineBuildingStrategy
    {
        List<Turn> GetIndexesToBuild(ICommand command);
    }
}
