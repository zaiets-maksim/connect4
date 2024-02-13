using System.Collections.Generic;
using Connect4.Scripts.Turns;

namespace Connect4.Scripts.Strategies.IBlockingStrategy
{
    public interface ILineBuildingStrategy
    {
        List<Turn> GetIndexesToBuild(ICommand command);
    }
}
