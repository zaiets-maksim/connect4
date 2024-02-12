using System.Threading.Tasks;
using UnityEngine;

namespace Connect4.Scripts.Commands
{
    public class BannedCommand : ICommand
    {
        public Vector2Int Index { get; }
        public Player ActivePlayer { get; }
        
        public BannedCommand(Vector2Int index, Player activePlayer)
        {
            ActivePlayer = activePlayer;
            Index = index;
        }

        public Task Execute() { return null; }

        public void Undo() { }

        public void ToHistory() { }
    }
}
