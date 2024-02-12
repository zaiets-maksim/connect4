using System.Collections.Generic;
using UnityEngine;

namespace Connect4.Scripts.Strategies.IBlockingStrategy
{
    public interface IBlockingStrategy
    {
        List<Vector2Int> GetIndexesToBlock(ICommand command);
    }
}
