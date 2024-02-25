using System.Threading.Tasks;
using UnityEngine;

namespace Connect4.Scripts.Commands
{
    public interface ICommand
    {
        public Vector2Int Index { get; }
        public Player.Player ActivePlayer { get; }
    
        public Task Execute();
        public void Undo();
        public void ToHistory();
    }
}