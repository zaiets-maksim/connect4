using System.Threading.Tasks;
using UnityEngine;

public interface ICommand
{
    public Vector2Int Index { get; }
    public Player ActivePlayer { get; }
    
    public Task Execute();
    public void Undo();
    public void ToHistory();
}