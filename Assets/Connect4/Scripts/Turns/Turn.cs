using UnityEngine;

namespace Connect4.Scripts.Turns
{
    public class Turn
    {
        public Vector2Int Index { get; }
        public int Priority { get; }

        public Turn(Vector2Int index, int priority = 0)
        {
            Priority = priority;
            Index = index;
        }
    }
}
